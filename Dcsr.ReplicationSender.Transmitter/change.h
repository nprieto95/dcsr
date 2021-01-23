#pragma once

#include <string>

class Change
{
public:
	Change(std::string delta);
	void execute();
};

class Deletion: public Change
{
public:
	Deletion(std::string delta);
};