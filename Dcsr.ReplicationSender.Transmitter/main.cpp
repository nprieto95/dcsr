#include <cstdio>
#include <ams.h>
#include <exception>
#include <string>
#include <limits.h>
#include <unistd.h>

char* getexepath()
{
	char result[PATH_MAX];
	ssize_t count = readlink("/proc/self/exe", result, PATH_MAX);
	return result;
}

static AmsModule me;

int main()
{
	int regustrationResult = ams_register("/home/nahuel/Dcsr/ION/Earth/mib.amsrc", NULL, "dcsr", "uade", "", "transmitter", &me);
	//printf("%d", regustrationResult);
	//printf(getexepath());
    return 0;
}