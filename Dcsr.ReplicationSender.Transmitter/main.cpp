#include <ams.h>

constexpr auto MARS_CONTINUUM = 2;
constexpr auto DELTA_SUBJECT = 2;

int main()
{
	AmsModule module;
	char mibSource[] = "/home/nahuel/Dcsr/ION/Earth/mib.amsrc";
	char applicationName[] = "dcsr";
	char authorityName[] = "uade";
	char unitName[] = "";
	char roleName[] = "transmitter";
	int registrationFailed = ams_register(mibSource, NULL, applicationName, authorityName, unitName, roleName, &module);
	if (!registrationFailed)
		writeMemo("Transmitter successfully registered.");
	else
		return -1;

	char message[] = "hello from the pale blue dot";
	while (true)
	{
		/*int publishingFailed = ams_publish(module, DELTA_SUBJECT, 1, 0, 27, message, 0);
		if (publishingFailed)
			break;
		sleep(30);*/
		int sendingFailed = ams_send(module, MARS_CONTINUUM, 0, 1, DELTA_SUBJECT, 1, 0, 28, message, 0);
		if (sendingFailed)
			break;
		sleep(30);
	}

	int unregistrationFailed = ams_unregister(module);
	if (!unregistrationFailed)
		writeMemo("Transmitter successfully unregistered.");
	else
		return -1;
	return 0;
}