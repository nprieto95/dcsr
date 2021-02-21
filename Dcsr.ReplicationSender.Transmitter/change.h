#pragma once

#include <string>
#include <cfdp.h>

#include "string_functions.h";

class Change
{
protected:
	std::string source_prefix = "/run/user/1000/gvfs/smb-share:server=main,share=dcsr";
	std::string destination_prefix = "/home/nahuel/Documents";
	CfdpNumber mars_cfdp_number;
	CfdpTransactionId transaction_id;
	virtual MetadataList get_filestore_requests() = 0;
public:
	virtual void apply() = 0;
	Change()
	{
		cfdp_compress_number(&mars_cfdp_number, 2);
	}
};

class TransferfulChange : public Change
{
private:
	std::string source;
protected:
	std::string destination;
	virtual std::string get_destination_file_name() = 0;
public:
	TransferfulChange(std::string file_name) : source(source_prefix + file_name), destination(destination_prefix + file_name) {};
	void apply() override
	{
		auto filestore_requests = get_filestore_requests();
		cfdp_put(
			&mars_cfdp_number,
			0, NULL,
			to_c_string(source),
			to_c_string(get_destination_file_name()),
			NULL, NULL, NULL, 0, NULL, 0, NULL, filestore_requests, &transaction_id);
	};
};

class FileCreation : public TransferfulChange
{
public:
	FileCreation(std::string file_name) : TransferfulChange(file_name) {};
protected:
	std::string get_destination_file_name() override
	{
		return destination;
	}
	MetadataList get_filestore_requests() override
	{
		auto requests = cfdp_create_fsreq_list();
		return requests;
	}
};

class FileModification : public TransferfulChange
{
public:
	FileModification(std::string file_name) : TransferfulChange(file_name) {};
protected:
	std::string get_destination_file_name() override
	{
		return destination + "new";
	}
	MetadataList get_filestore_requests() override
	{
		auto requests = cfdp_create_fsreq_list();
		// File overwriting is not directly supported, so we create a different one with a new name, delete the existing one and then rename the new one.
		cfdp_add_fsreq(requests, CfdpAction::CfdpDenyFile, to_c_string(destination), NULL);
		cfdp_add_fsreq(requests, CfdpAction::CfdpRenameFile, to_c_string(get_destination_file_name()), to_c_string(destination));
		return requests;
	}
};

class TransferlessChange : public Change
{
public:
	void apply() override
	{
		auto filestore_requests = get_filestore_requests();
		cfdp_put(
			&mars_cfdp_number,
			0, NULL,
			NULL, // source
			NULL, // destination
			NULL, NULL, NULL, 0, NULL, 0, NULL, filestore_requests, &transaction_id);
	};
};

class FileRenaming : public TransferlessChange
{
private:
	std::string old_file_name;
	std::string new_file_name;
public:
	FileRenaming(std::string old_file_name, std::string new_file_name) : old_file_name(destination_prefix + old_file_name), new_file_name(destination_prefix + new_file_name) {};
protected:
	MetadataList get_filestore_requests() override
	{
		auto requests = cfdp_create_fsreq_list();
		cfdp_add_fsreq(requests, CfdpAction::CfdpRenameFile, to_c_string(old_file_name), to_c_string(new_file_name));
		return requests;
	};
};

class FileDeletion : public TransferlessChange
{
private:
	std::string file_name;
public:
	FileDeletion(std::string file_name) : file_name(destination_prefix + file_name){};
protected:
	MetadataList get_filestore_requests() override
	{
		auto requests = cfdp_create_fsreq_list();
		cfdp_add_fsreq(requests, CfdpAction::CfdpDenyFile, to_c_string(file_name), NULL);
		return requests;
	};
};