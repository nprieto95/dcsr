#include <iostream>
#include <string>

#include <was/storage_account.h>
using utility::string_t;
using namespace azure::storage;
#include <was/queue.h>


#include <ams.h>

#include "IoFunctions.h"
#include "AmsFunctions.h"
#include "StringFunctions.h"

using namespace std;

int main()
{
    AmsModule amsModule;
    auto registrationResult = register_module(&amsModule);
    if (registrationResult == 0)
        cout << "Registration successful." << endl;
    else
        return registrationResult;

    // Message sending
    int subjectNbr = ams_lookup_subject_nbr(amsModule, "delta");

    // Define the connection-string with your values.
    const string_t storage_connection_string(U("DefaultEndpointsProtocol=https;AccountName=dcsrreplicationsender;AccountKey=uxLmzSSIxediAAzH4oiohM+aE/cBpnuAA3dNAonK9sYUSb1KMkxsqWlU8YwqbkJpR5JvJayn5di8FC5Sb4j+5A==;BlobEndpoint=https://dcsrreplicationsender.blob.core.windows.net/;QueueEndpoint=https://dcsrreplicationsender.queue.core.windows.net/;TableEndpoint=https://dcsrreplicationsender.table.core.windows.net/;FileEndpoint=https://dcsrreplicationsender.file.core.windows.net/;"));

    // Retrieve storage account from connection string.
    cloud_storage_account storage_account = cloud_storage_account::parse(storage_connection_string);

    // Create the queue client.
    cloud_queue_client queue_client = storage_account.create_cloud_queue_client();

    // Retrieve a reference to a queue.
    cloud_queue queue = queue_client.get_queue_reference(U("deltas"));

    string message = "";
    int publishingResult;
    while (get_message(message, "Retrieve next message? <anything>/n") != "n")
    {

        // Get the next message.
        cloud_queue_message dequeued_message = queue.get_message();
        string dequeued_message_string = dequeued_message.content_as_string();

        publishingResult = ams_publish(amsModule, subjectNbr, 0, 0, dequeued_message_string.length(), to_c_string(dequeued_message_string), 0);
        if (publishingResult == 0)
        {
            cout << "Published successfully." << endl;
            // Delete the message.
            queue.delete_message(dequeued_message);
        }
        else
            return publishingResult;

    }

    return 0;

}