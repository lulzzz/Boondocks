# Device API v1
This is the RESTful api that the Agent communicates with.

# 1. Brief Introduction
- This is a REST API. 

# 2. Errors
The Engine API uses standard HTTP status codes to indicate the success or failure of the API call. The body of the response will be JSON in the following format:

    {
        "message": "page not found"
    }

The status codes that are returned for each endpoint are specified in the endpoint documentation below.

# 3. Endpoints

## 3.1 AgentDownloadInfo

Acquire agent download information.

`POST /v1/agentDownloadInfo`

Example request:

```
POST /v1/agentDownloadInfo
```

with body (specify the id of the agent version to be downloaded)

```
{
   "id" : "ac9a0159-0842-4bbb-8d82-fdb6b99cb14b"
}
```

Example response:

```
{
    "imageId" : "sha256:ca1f5f48ef431c0818d5e8797dfe707557bdc728fe7c3027c75de18f934a3b76",
    "registry" : "10.0.5.15:5000",
    "authToken" : "",
    "repository" : "agent",
    "name" : "v1.0.0
}
```

## 3.2 ApplicationDownloadInfo
Acquire application download information.

`POST /v1/applicationDownloadInfo`

Example request:

`POST /v1/applicationDownloadInfo`

with body (specify the id of the application version to be downloaded)

```
{
    "id" : "3332BA33-1B30-4BA3-9D25-E96ADD9A8731"
}
```

Example response:

```
HTTP/1.1 200 OK
Content-Type: application/json

{
    "imageId" : "sha256:ca1f5f48ef431c0818d5e8797dfe707557bdc728fe7c3027c75de18f934a3b76",
    "registry" : "10.0.5.15:5000",
    "authToken" : "",
    "repository" : "9447e6d9-e3be-463c-a37f-f3b46f2097d7",
    "name" : "v2.0.1
}
```

## 3.3 DeviceConfiguration

`GET v1/deviceConfiguration`

Example response:

```
HTTP/1.1 200 OK
Content-Type: application/json

{
    "agentVersion" : {
        "id" : "84a5490c-6a81-4876-8f8b-cddf1d83c6bb",
        "imageId" : "sha256:d736f8a1-e1f2-454a-8f74-d52ff4151d9d",
        "name" : "v1.0.0"
    },
    "rootFileSystemVersionId" : null,
    "environmentVariables" : [
        {
            "name" : "ENVIRONMENT_VAR_1",
            "value" : "VALUE1"
        }   
    ],
    "configurationVersion" : "2f0ca20d-0eec-442b-88f5-da0f6fe7fd47"
    
}
```

## 3.4 Heartbeat

`POST v1/heartbeat`

Example request:

`POST /v1/heartbeat`

Example body:

```
{
   "uptimeSeconds" : 42,
   "agentVersion" : "sha256:d74508fb6632491cea586a1fd7d748dfc5274cd6fdfedee309ecdcbc2bf5cb82",
   "applicationVersion" : "sha256:d74508fb6632491cea586a1fd7d748dfc5274cd6fdfedee309ecdcbc2bf5cb82",
   "rootFileSystemVersion" : null,
   "state" : 0
}
```

JSON parameters:
- **uptimeSeconds** - the number of seconds the agent has been running.
- **agentVersion** - the ImageID of the curently running agent container.
- **applicationVersion** - the ImageID of the curently running application container.
- **rootFileSystemVersion** - information about the version of the root file system in use.

Example response:

```
HTTP/1.1 200 OK
Content-Type: application/json

{
   "configuration-version" : "ec69da52-2cc8-4560-bc75-e77d572a92f6"
}

```

JSON response:
- **configuration-version** - An arbitrary id that is updated whenever the effective configuration for a device may have changed. This is done so that we don't have to keep polling for our current configuration (an expensive process).

