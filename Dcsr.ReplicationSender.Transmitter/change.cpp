#include "change.h"

using namespace std;

#include <boost/json.hpp>
using boost::json::parse;
using boost::json::value;

Change::Change(string delta)
{
	value jv = parse(delta);
}

Deletion::Deletion(string delta) :Change(delta) {}