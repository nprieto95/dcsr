#include <iostream>
#include <string>
#include <algorithm>

#include <boost/json.hpp>
using boost::json::parse;
using boost::json::value;
#include <boost/algorithm/string.hpp>
using boost::split;

#include <cfdp.h>

#include "storage_functions.h"
#include "string_functions.h"
#include "change.h"

using namespace std;

int main()
{

	auto queue = get_queue();

	string share_prefix = "/run/user/1000/gvfs/smb-share:server=main,share=dcsr";
	string dest_prefix = "/home/nahuel/Documents";

	do
	{

		queue.download_attributes();

		auto approximate_message_count = queue.approximate_message_count();

		if (approximate_message_count == 0)
		{
			cout << "No messages detected this time. Will poll again in 10 seconds." << endl;
			continue;
		}
		// Only required if working directly from SharePoint.
		/*else
		{
			cout << "Allowing 60 seconds to pass for the local SMB share to get updated." << endl;
			sleep(60);
		}*/

		auto messages = queue.get_messages(approximate_message_count);

		for (auto message : messages)
		{
			auto message_string = message.content_as_string();
			value jv = parse(message_string);
			if (jv.is_array())
			{
				auto attach_result = cfdp_attach();
				if (attach_result == -1)
				{
					cout << "Couldn't attach to CFDP. Will retry in 10 seconds..." << endl;
					goto outer_continue;
				}
				auto items = jv.as_array();
				for (auto item : items)
				{

					auto o = item.as_object();

					if (o.contains("file"))
					{

						CfdpNumber mars_cfdp_nbr;
						CfdpTransactionId transaction_id;
						cfdp_compress_number(&mars_cfdp_nbr, 2);
						auto requests = cfdp_create_fsreq_list();

						string file_name = o["name"].as_string().c_str();
						string raw_file_path = o["parentReference"].as_object()["path"].as_string().c_str();
						string prior_raw_file_path = "*";
						string prior_file_name = "*";

						if (file_name.find('*') != string::npos)
						{
							vector<string> split_results;
							split(split_results, file_name, [](char c) {return c == '*'; });
							prior_file_name = split_results[0];
							file_name = split_results[1];
						}

						if (raw_file_path.find('*') != string::npos)
						{
							vector<string> split_results;
							split(split_results, raw_file_path, [](char c) {return c == '*'; });
							prior_raw_file_path = split_results[0];
							raw_file_path = split_results[1];
						}

						string file_path = raw_file_path.substr(raw_file_path.find(':') + 1);
						string prior_file_path = prior_raw_file_path == "*" ? "*" : prior_raw_file_path.substr(prior_raw_file_path.find(':') + 1);
						string file_path_source = share_prefix + file_path + '/' + file_name;
						string file_path_destination = dest_prefix + file_path + '/' + file_name;
						string prior_file_path_destination = "*";

						if (prior_file_path != "*" || prior_file_name != "*")
							prior_file_path_destination = dest_prefix +
							(prior_file_path != "*" ? prior_file_path : file_path) + '/' +
							(prior_file_name != "*" ? prior_file_name : file_name);

						int put_result;

						if (!o.contains("deleted"))
						{

							if (prior_file_path_destination != "*")
							{
								// This is a rename
								cout << "Renaming " << prior_file_path_destination << " to " << file_path_destination << endl;
								cfdp_add_fsreq(requests, CfdpAction::CfdpRenameFile, to_c_string(prior_file_path_destination), to_c_string(file_path_destination));
								put_result = cfdp_put(
									&mars_cfdp_nbr,
									0,
									NULL,
									NULL,
									NULL,
									NULL, NULL, NULL, 0, NULL, 0, NULL, requests, &transaction_id);
							}
							else
							{
								cout << "Sending " << file_name << " to Mars." << endl;
								cfdp_add_fsreq(requests, CfdpAction::CfdpDenyFile, to_c_string(file_path_destination), NULL);
								cfdp_add_fsreq(requests, CfdpAction::CfdpRenameFile, to_c_string(file_path_destination + "new"), to_c_string(file_path_destination));
								put_result = cfdp_put(
									&mars_cfdp_nbr,
									0,
									NULL,
									to_c_string(file_path_source),
									to_c_string(file_path_destination + "new"),
									NULL, NULL, NULL, 0, NULL, 0, NULL, requests, &transaction_id);
							}
						}
						else
						{
							cout << "Deleting " << file_name << " on Mars." << endl;
							cfdp_add_fsreq(requests, CfdpAction::CfdpDenyFile, to_c_string(file_path_destination), NULL);
							put_result = cfdp_put(
								&mars_cfdp_nbr,
								0,
								NULL,
								NULL,
								NULL,
								NULL, NULL, NULL, 0, NULL, 0, NULL, requests, &transaction_id);
						}

						if (put_result == -1)
						{
							cout << "Something went wrong. Will retry the batch in 10 seconds." << endl;
							goto outer_continue;
						}

					}

				}
				cfdp_detach();
			}

			queue.delete_message(message);

		}

	outer_continue:
		continue;

	} while (!sleep(10));

	return 0;

}