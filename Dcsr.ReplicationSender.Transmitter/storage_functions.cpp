#include <was/storage_account.h>
using utility::string_t;
using namespace azure::storage;
#include <was/queue.h>

cloud_queue get_queue()
{
    // Define the connection-string with your values.
    const string_t storage_connection_string(U("DefaultEndpointsProtocol=https;AccountName=dcsrreplicationsender;AccountKey=uxLmzSSIxediAAzH4oiohM+aE/cBpnuAA3dNAonK9sYUSb1KMkxsqWlU8YwqbkJpR5JvJayn5di8FC5Sb4j+5A==;BlobEndpoint=https://dcsrreplicationsender.blob.core.windows.net/;QueueEndpoint=https://dcsrreplicationsender.queue.core.windows.net/;TableEndpoint=https://dcsrreplicationsender.table.core.windows.net/;FileEndpoint=https://dcsrreplicationsender.file.core.windows.net/;"));

    // Retrieve storage account from connection string.
    cloud_storage_account storage_account = cloud_storage_account::parse(storage_connection_string);

    // Create the queue client.
    cloud_queue_client queue_client = storage_account.create_cloud_queue_client();

    // Retrieve a reference to a queue.
    cloud_queue queue = queue_client.get_queue_reference(U("deltas"));

    return queue;
}