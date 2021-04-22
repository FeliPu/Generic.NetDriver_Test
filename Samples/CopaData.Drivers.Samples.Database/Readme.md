# Access to an MS SQL database using the Generic.Net Driver

This project demonstrates the access to a MS SQL database. 

## Prerequisites

In order for this example to run you should prepare a __database__. Check the connecion string in the DbManager class for an example database name (called MerckpolTest). The database has exactly one table (called 'AllSimulationData') with two columns (called 'Id' and 'Value').

The connection string needs to be adapted according to your needs. Also, the table should be filled with some data. Example:

| Id | Value |
| --- | --- |
|1|Value 1|
|2|Value 2|
|3|Value 3|

The **zenon project** (in version 10) could be very basic. A simple screen with one dynamic element displaying one string variable of the GenericNet driver is sufficient. Please remark the configuration of the GenericNet driver and the configuration of the symbolic address of the varaible. 

Requesting data from the database starts when a value is written to that string variable.


## Crux
Beeing a .Net Core 3.1 class library, the driver extension makes use of the Microsoft.Data.SqlClient (v2.1.2) nuget package. 

There is however a problem when integrating the nuget packet as the packet manager manages to copy the correct .dlls in the output directory but they remain unfound by the driver extension.

This results in exceptions like `"Microsoft.Data.SqlClient is not supported on this platform"` or `Strings.PlatformNotSupported_DataSqlClient`.

## Solution

To overcome these exceptins, the following .dlls should be added directly to the project:
+ `Microsoft.Data.SqlClient.dll`
+ `Microsoft.Data.SqlClient.SNI.dll`

Adding the dlls is easy done by adding the following lines of XML to the .cproj file:

``` XML
<!-- For publish -->
  <ItemGroup>
    <None Include="$(USERPROFILE)\.nuget\packages\microsoft.data.sqlclient\2.1.2\runtimes\win\lib\netcoreapp2.1\Microsoft.Data.SqlClient.dll">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>
  <ItemGroup>
    <None Include="$(USERPROFILE)\.nuget\packages\microsoft.data.sqlclient.sni\2.1.1\build\net46\Microsoft.Data.SqlClient.SNI.x86.dll">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>
  <!-- For local debug -->
  <Target Name="CopyToBin" BeforeTargets="Build">
    <Copy SourceFiles="$(USERPROFILE)\.nuget\packages\microsoft.data.sqlclient\2.1.2\runtimes\win\lib\netcoreapp2.1\Microsoft.Data.SqlClient.dll" DestinationFolder="$(OutputPath)\bin" />
  </Target>
  <Target Name="CopyToBin" BeforeTargets="Build">
    <Copy SourceFiles="$(USERPROFILE)\.nuget\packages\microsoft.data.sqlclient.sni\2.1.1\build\net46\Microsoft.Data.SqlClient.SNI.x86.dll" DestinationFolder="$(OutputPath)\bin" />
  </Target>
```

There is one thing however, the `Microsoft.Data.SqlClient.SNI.x86.dll` is added as x86 or x64 respectively. In order for the driver extension to work the dll needs to be available without the x86 or x64 part. Therfore, a post build command has been added to the project to rename the dll accordingly:

`rename "$(registry:HKEY_LOCAL_MACHINE\SOFTWARE\COPA-DATA\DataDir@ProgramDir32_10000)DriverExtensions\Database\Microsoft.Data.SqlClient.SNI.x86.dll" "Microsoft.Data.SqlClient.SNI.dll"
`

## Kudos
_Kudos_ to Youness Radi for the good preparation work.

_Kudos_ to Hiral Desai for pointing me in the right direction.
 https://www.gitmemory.com/issue/Azure/azure-functions-host/5423/569552658



