# BasAutonumber Solution

This solution enables you to configure autonumbering for a Table in Dataverse. This will take in any prefix/suffix you choose and guarantee a unique custom ID field (string) is generated for you. It may be configured for any Table in the system provided it enables plug-in registration against it.

You may configure:
- Prefix and suffix for your generated autonumber
- Increment by how many units
- What's the starting ID
- Length of the generated autonumber

This solution has been tested included heavy loads of record creation (thousands of records created simultaneously) to ensure uniqueness in the generated autonumber.

## Development
This solution was developed as an alternative to other autonumbering solutions out there. The reason I've developed this algorithm was because most of the auton-umbering solutions out there weren't properly implementing a lock to block concurrent calls (meaning the generated autonumber was not 100% unique), and the solutions which did implement this feature at the time of development were not free.

To achieve this goal, unsecure configuration is leveraged to dynamically store the GUID of a particular autonumbering record. This GUID is then used to run an update message against the record, which forces SQL to open a transaction and stop any subsequent operations on that record until after the numbering has been incremented.

A TO-DO here would be to automate the creation of the plug-in step for autonumbering on a given table (triggered when you create the autonumber record).


## To configure autonumbering for one of your Tables


1. Deploy the BasAutoNumber solution (managed and unmanaged are available on this repository)


2. Create a configuration record in the BasAutoNumber table. Fill in all required fields:
    - **Name** - Record name
    - **Entity Name** - Entity name where autonumber will execute on
    - **Field Name** - Field where the auto generated number will be placed
    - **Increment By** - Quantity that will increase with the next auto generated number
    - **Starting ID** -  Staring number of the autonumber sequence
    - **Next ID** - Next number of autonumber
    - **Length** - Length of the auto generated number
    - **Prefix** - Autonumber prefix [Optional]
    - **Suffix** – Autonumber suffix [Optional]



3.	Create a plug-in step for the Entity where autonumber is being configured:
    - Open the Plugin Registration Tool;
    - Select the Bas.CRM.BasAutoNumber assembly;
    - Right-click on the Plugin and select Register New Step;
    - Fill in the standard fields of a plug-in step (Image 2). In Primary Entity fill in the Entity where autonumber will execute on;
    - Finally, click on Register New Step or Update Step button;
 


4. To ensure unique sequence number against concurrent calls, an additional setting is necessary. On the Unsecure Configuration section, update the XML with the GUID of the configuration record created on step 1 of this document. E.g.: <LockGuid> cb791047-7e10-4324-a817-1400aa0ae348 </LockGuid>


## Disclaimer
This code has initially been developed in 2017 and has recently been shared on my personal Github.
