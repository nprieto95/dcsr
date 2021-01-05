#include <string>
#include <cstring>
using namespace std;

char* to_c_string(string str)
{
	return strdup(str.c_str());
}