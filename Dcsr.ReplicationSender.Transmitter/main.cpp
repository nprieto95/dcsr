#include <cstdio>
#include <ams.h>

static AmsModule me;

int main()
{
    ams_register("~/Dcsr/ION/Earth/mib.amsrc", NULL, NULL, NULL, 0, "amsdemo", "test", "",
        "pitch", &me);
    return 0;
}