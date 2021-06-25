# TaskTrackingApp
Brief description: the tool helps the managers to monitor the status of bugs and tasks easily. We can consider it as a efficient communication channel to advance team's performance.

# Running this project
* Make sure there is an App_Data folder in the project (Right click solution > View in File Explorer)
* Tools > Nuget Package Manager > Package Manage Console > Update-Database
* Check that the database is created using (View > SQL Server Object Explorer > MSSQLLocalDb > ..)
## Run API commands through CURL to run CRUD operations
Make sure to utilize jsondata/developer.json to formulate data you wish to send as part of the POST requests. {id} should be replaced with the developer's primary key ID when running update operation, but it should be removed for adding a new developer. The port number may not always be the same.
Get a list of developers curl https://localhost:44382/api/developerdata/listdevelopers

Get a developer curl https://localhost:44382/api/developerdata/finddeveloper/{id}

Add a new developer (new animal info is in developer.json but DevID must be removed from json file) curl -H "Content-Type:application/json" -d @developer.json https://localhost:44382/api/developerdata/adddeveloper

Delete a developer curl -d "" https://localhost:44382/api/developerdata/deletedeveloper/{id}

Update a developer (existing developer info including id must be included in developer.json) curl -H "Content-Type:application/json" -d @developer.json https://localhost:44382/api/developerdata/updatedeveloper/{id}

## Running the Views for List, Details, New
Use SQL Server Object Explorer to add a new Developer

Take note of the Developer ID

Navigate to /Developer/New

Input the First nam, last name and position

Click "Add"

Navigate to /Developer/List

Navigate to Developer/Details/{developer id}

# ERD and relationships:
![image](https://user-images.githubusercontent.com/12003260/123426301-446e8500-d591-11eb-873c-91deac1a86c3.png)
![image](https://user-images.githubusercontent.com/12003260/123426355-518b7400-d591-11eb-8a80-8b8a81a81a07.png)

# My initial wireframes
![image](https://user-images.githubusercontent.com/12003260/123426401-60722680-d591-11eb-8429-f6f1c0eb8d61.png)

A Developer information interface. The base information about the developer can be modified such as name, position. The part of assigned tasks is the relationship between a developer and their tasks. This relationship is read-only on this interface.

![image](https://user-images.githubusercontent.com/12003260/123426478-767fe700-d591-11eb-951b-d3c0c429ef10.png)

A Task Information interface. A task is only assigned to a developer. Due to ERD, there are many tasks that can be assigned to a developer. We can choose a developer from the drop-down list to assign tasks. When assigning a task, we must change the status to “In progress”. Besides, there are some priorities in the drop-down list such as Blocker, Critical, Major, Minor, Trivial. Additionally, we can update the description of a task

# My final website
## Interfaces of assignment relates to CRUD operations
### List of assignments
![image](https://user-images.githubusercontent.com/12003260/123426753-c9599e80-d591-11eb-854d-7a2aa568d405.png)
### Create a new assignment (Create operation)
![image](https://user-images.githubusercontent.com/12003260/123427278-6c121d00-d592-11eb-965b-07d1091834ac.png)
### Detail of a particular assignment (Read operation)
![image](https://user-images.githubusercontent.com/12003260/123426831-e1312280-d591-11eb-8147-c390b4b49af3.png)
### Update interface of a particular assignment (Update operation)
![image](https://user-images.githubusercontent.com/12003260/123426916-fa39d380-d591-11eb-862c-3a6403bc1320.png)
### Delete confirmation interface (Delete operation)
![image](https://user-images.githubusercontent.com/12003260/123427056-248b9100-d592-11eb-914a-b92a4eb99044.png)

## Interfaces of developer relates to CRUD operations
### List of developers
![image](https://user-images.githubusercontent.com/12003260/123427739-04100680-d593-11eb-9ad6-6709337f4814.png)
### Add a new developer (Create operation)
![image](https://user-images.githubusercontent.com/12003260/123427773-0c684180-d593-11eb-97dd-bf04eaadb679.png)
### Detail of a particular developer (Read operation)
![image](https://user-images.githubusercontent.com/12003260/123427880-2b66d380-d593-11eb-8c9c-4dcb95378998.png)
### Update interface of a particular developer (Update operation)
![image](https://user-images.githubusercontent.com/12003260/123427914-36216880-d593-11eb-9fa1-0732542abdbb.png)
### Delete confirmation interface (Delete operation)
![image](https://user-images.githubusercontent.com/12003260/123427964-45a0b180-d593-11eb-9c70-f44f2fb1961f.png)

## Interfaces of skill relates to CRUD operations
### List of skills
![image](https://user-images.githubusercontent.com/12003260/123428020-52bda080-d593-11eb-874d-120d876ed77f.png)
### Create a new skill (Create operation)
![image](https://user-images.githubusercontent.com/12003260/123428041-5cdf9f00-d593-11eb-82e9-37fb4b4bfea4.png)
### Detail of a particular skill (Read operation)
![image](https://user-images.githubusercontent.com/12003260/123429115-8fd66280-d594-11eb-81cc-0e5560d14c5b.png)
### Update interface of a particular skill (Update operation)
![image](https://user-images.githubusercontent.com/12003260/123428470-daa3aa80-d593-11eb-9a6f-e3b323babeb4.png)
### Delete confirmation interface (Delete operation)
![image](https://user-images.githubusercontent.com/12003260/123429178-a5e42300-d594-11eb-9423-f297d662b1e1.png)


# References:
(almost things I learned to develop this project are through a series of Zoo Applications. You can check out this repo for more information)
https://github.com/christinebittle/ZooApplication_4 

https://docs.microsoft.com/en-us/aspnet/mvc/

https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application

# Extra features in future:
* Image upload for developer profile/details
* Simple code review feature
* Progress feature includes comments section in task detail and the way to show how many percent a task is finished (progress bar, for example)
* Improve user interface (home page, about page) with more interesting content.
