USE [yourdb]
GO

/****** Object:  UserDefinedTableType [dbo].[weshipexpresspro_temp_shipstransactions]    Script Date: 19/11/2024 16:45:27 ******/
CREATE TYPE [dbo].[weshipexpresspro_temp_shipstransactions] AS TABLE(
	[idKey] [INT]  NOT NULL,
	[orderNumber] [VARCHAR](255) NULL,
	[status] [INT] NULL,
	[createdOn] [DATETIME] NULL,
	[updatedOn] [DATETIME] NULL,
	[reason] [VARCHAR](255) NULL,
	[orderStatusDesc] [VARCHAR](255) NULL,
	[statusSequence] [INT] NULL,
	[shipToCity] [VARCHAR](255) NULL,
	[shipToState] [VARCHAR](255) NULL
)
GO





-----------------------------------------------------------------------------------------------------------------------
USE [yourdb]
GO

/****** Object:  UserDefinedTableType [dbo].[weshipexpresspro_temp_ships]    Script Date: 25/11/2024 18:25:15 ******/
CREATE TYPE [dbo].[weshipexpresspro_temp_ships] AS TABLE(
	[idKey] [INT]  NOT NULL,
	[id] [VARCHAR](255) NULL,
	[createdOn] [BIGINT] NULL,
	[updatedOn] [BIGINT] NULL,
	[parentOrder] [VARCHAR](255) NULL,
	[orderInfoOrderType] [INT] NULL,
	[orderTypeDesc] [VARCHAR](255) NULL,
	[status] [INT] NULL,
	[orderStatusDesc] [VARCHAR](255) NULL,
	[accountId] [VARCHAR](255) NULL,
	[clientName] [VARCHAR](255) NULL,
	[companyName] [VARCHAR](255) NULL,
	[shipperId] [INT] NULL,
	[warehouseLocation] [VARCHAR](255) NULL,
	[updatedBy] [INT] NULL,
	[totalQuantity] [INT] NULL,
	[totalWeight] [FLOAT] NULL,
	[paymentMode] [INT] NULL,
	[paymentStatus] [INT] NULL,
	[serviceType] [VARCHAR](255) NULL,
	[shippingMode] [VARCHAR](255) NULL,
	[lob] [INT] NULL,
	[lobStatus] [VARCHAR](255) NULL,
	[storeId] [VARCHAR](255) NULL,
	[storeName] [VARCHAR](255) NULL,
	[channel] [VARCHAR](255) NULL,
	[referenceId] [VARCHAR](255) NULL,
	[deliveryTargetDate] [BIGINT] NULL,
	[deliveryTargetTime] [VARCHAR](255) NULL,
	[trackingId] [VARCHAR](255) NULL,
	[internalTrackingId] [VARCHAR](255) NULL,
	[boxId] [VARCHAR](255) NULL,
	[boxCount] [INT] NULL,
	[orderPod] [VARCHAR](255) NULL,
	[carrier] [VARCHAR](255) NULL,
	[labelPrinted] [INT] NULL,
	[notification] [BIT] NULL,
	[addInfoCompanyName] [VARCHAR](255) NULL,
	[facilityName] [VARCHAR](255) NULL,
	[beginDate] [VARCHAR](255) NULL,
	[reason] [VARCHAR](255) NULL,
	[isAsnGenerated] [BIT] NULL,
	[orderState] [INT] NULL,
	[otm] [INT] NULL,
	[carrierAccountId] [VARCHAR](255) NULL,
	[shipmentType] [VARCHAR](255) NULL,
	[customerId] [VARCHAR](255) NULL,
	[isFulfilment] [INT] NULL,
	[label] [INT] NULL,
	[labelUrl] [VARCHAR](MAX) NULL,
	[isCCP] [BIT] NULL,
	[shipFromName] [VARCHAR](255) NULL,
	[shipFromAddress] [VARCHAR](255) NULL,
	[shipFromCity] [VARCHAR](255) NULL,
	[shipFromState] [VARCHAR](255) NULL,
	[shipFromCountry] [VARCHAR](255) NULL,
	[shipFromPostalCode] [VARCHAR](255) NULL,
	[shipFromEmail] [VARCHAR](255) NULL,
	[shipFromMobile] [VARCHAR](255) NULL,
	[shipToName] [VARCHAR](255) NULL,
	[shipToAddress] [VARCHAR](255) NULL,
	[shipToAddress2] [VARCHAR](255) NULL,
	[shipToCity] [VARCHAR](255) NULL,
	[shipToState] [VARCHAR](255) NULL,
	[shipToCountry] [VARCHAR](255) NULL,
	[postalCode] [VARCHAR](255) NULL,
	[shipToEmail] [VARCHAR](255) NULL,
	[shipToMobile] [VARCHAR](255) NULL,
	[billToName] [VARCHAR](255) NULL,
	[billToAddress] [VARCHAR](255) NULL,
	[billToAddress2] [VARCHAR](255) NULL,
	[billToCity] [VARCHAR](255) NULL,
	[billToState] [VARCHAR](255) NULL,
	[billToCountry] [VARCHAR](255) NULL,
	[billToPostal] [VARCHAR](255) NULL,
	[billToEmail] [VARCHAR](255) NULL,
	[billToMobile] [VARCHAR](255) NULL,
	[containerizedOrder] [BIT] NULL,
	[multiItemOrders] [INT] NULL,
	[shipstationOrder] [INT] NULL,
	[updateCarrier] [BIT] NULL,
	[rateCardEnable] [BIT] NULL,
	[batchGenerated] [BIT] NULL,
	[orderType] [VARCHAR](255) NULL,
	[icePack] [INT] NULL,
	[orderDate] [BIGINT] NULL,
	[ssPackingSlip] [INT] NULL,
	[ssShippingLabel] [INT] NULL,
	[productTypeDesc] [VARCHAR](255) NULL,
	[carrierLabelUrl] [VARCHAR](255) NULL,
	[connectorEmailStatus] [INT] NULL,
	[skuGroup] [VARCHAR](255) NULL,
	[fulfilment] [VARCHAR](255) NULL
)
GO







------------------------------------------------------------------------------------------------------
USE [yourdb]
GO

/****** Object:  StoredProcedure [dbo].[WEShipExpressTransactions_importData_20241111]    Script Date: 19/11/2024 16:44:07 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[WEShipExpressTransactions_importData_20241111]
    @DataToInsertTransaction dbo.weshipexpresspro_temp_shipstransactions READONLY
AS 
BEGIN
    PRINT 'Inizio popolamento TempTableDataShipTransaction';

    -- Check if the temp table exists and has data
    IF EXISTS (SELECT 1 FROM tempdb.sys.objects WHERE name = '##TempTableDataShipTransactions')
    BEGIN
        -- Determine the starting idKey value
        DECLARE @StartIdKey INT = ISNULL((SELECT MAX(idKey) FROM ##TempTableDataShipTransactions), 0);

        -- Insert data with incremented idKey
        INSERT INTO ##TempTableDataShipTransactions 
        (
            [idKey],
            [orderNumber],
            [status],
            [createdOn],
            [updatedOn],
            [reason],
            [orderStatusDesc],
            [statusSequence],
            [shipToCity],
            [shipToState]
        )
        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) + @StartIdKey AS [idKey],
            [orderNumber],
            [status],
            [createdOn],
            [updatedOn],
            [reason],
            [orderStatusDesc],
            [statusSequence],
            [shipToCity],
            [shipToState]
        FROM @DataToInsertTransaction;
    END
    ELSE
    BEGIN
        PRINT 'Temporary table ##TempTableDataShipTransactions does not exist. Creating the table...';

        -- Create the temp table if it doesn't exist
        CREATE TABLE ##TempTableDataShipTransactions
        (
            [idKey] INT NOT NULL,
            [orderNumber] NVARCHAR(50),
            [status] INT,
            [createdOn] DATETIME,
            [updatedOn] DATETIME,
            [reason] NVARCHAR(255),
            [orderStatusDesc] NVARCHAR(255),
            [statusSequence] INT,
            [shipToCity] NVARCHAR(100),
            [shipToState] NVARCHAR(100)
        );

        -- Insert data with idKey starting from 1
        INSERT INTO ##TempTableDataShipTransactions 
        (
            [idKey],
            [orderNumber],
            [status],
            [createdOn],
            [updatedOn],
            [reason],
            [orderStatusDesc],
            [statusSequence],
            [shipToCity],
            [shipToState]
        )
        SELECT
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [idKey],
            [orderNumber],
            [status],
            [createdOn],
            [updatedOn],
            [reason],
            [orderStatusDesc],
            [statusSequence],
            [shipToCity],
            [shipToState]
        FROM @DataToInsertTransaction;
    END;

    PRINT 'TempTableDataShipTransaction popolata con successo';
END;
GO




------------------------------------------------------------------------------------------------
 

CREATE PROCEDURE [dbo].[WEShipExpress_importData_20241111]
    @DataToInsert dbo.weshipexpresspro_temp_ships READONLY
AS 
BEGIN
    PRINT 'Inizio popolamento TempTableDataShip';

    -- Check if the temp table exists and has data
    IF EXISTS (SELECT 1 FROM tempdb.sys.objects WHERE name = '##TempTableDataShip')
    BEGIN
        -- Determine the starting idKey value
        DECLARE @StartIdKey INT = ISNULL((SELECT MAX(idKey) FROM ##TempTableDataShip), 0);

        -- Insert data with incremented idKey
        INSERT INTO ##TempTableDataShip 
        (
            [idKey],
            [id],
            [createdOn],
            [updatedOn],
            [parentOrder],
            [orderInfoOrderType],
            [orderTypeDesc],
            [status],
            [orderStatusDesc],
            [accountId],
            [clientName],
            [companyName],
            [shipperId],
            [warehouseLocation],
            [updatedBy],
            [totalQuantity],
            [totalWeight],
            [paymentMode],
            [paymentStatus],
            [serviceType],
            [shippingMode],
            [lob],
            [lobStatus],
            [storeId],
            [storeName],
            [channel],
            [referenceId],
            [deliveryTargetDate],
            [deliveryTargetTime],
            [trackingId],
            [internalTrackingId],
            [boxId],
            [boxCount],
            [orderPod],
            [carrier],
            [labelPrinted],
            [notification],
            [addInfoCompanyName],
            [facilityName],
            [beginDate],
            [reason],
            [isAsnGenerated],
            [orderState],
            [otm],
            [carrierAccountId],
            [shipmentType],
            [customerId],
            [isFulfilment],
            [label],
            [labelUrl],
            [isCCP],
            [shipFromName],
            [shipFromAddress],
            [shipFromCity],
            [shipFromState],
            [shipFromCountry],
            [shipFromPostalCode],
            [shipFromEmail],
            [shipFromMobile],
            [shipToName],
            [shipToAddress],
            [shipToAddress2],
            [shipToCity],
            [shipToState],
            [shipToCountry],
            [postalCode],
            [shipToEmail],
            [shipToMobile],
            [billToName],
            [billToAddress],
            [billToAddress2],
            [billToCity],
            [billToState],
            [billToCountry],
            [billToPostal],
            [billToEmail],
            [billToMobile],
            [containerizedOrder],
            [multiItemOrders],
            [shipstationOrder],
            [updateCarrier],
            [rateCardEnable],
            [batchGenerated],
            [orderType],
            [icePack],
            [orderDate],
            [ssPackingSlip],
            [ssShippingLabel],
            [productTypeDesc],
            [carrierLabelUrl],
            [connectorEmailStatus],
            [skuGroup],
            [fulfilment]
        )
        SELECT 
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) + @StartIdKey AS [idKey],
            [id],
            [createdOn],
            [updatedOn],
            [parentOrder],
            [orderInfoOrderType],
            [orderTypeDesc],
            [status],
            [orderStatusDesc],
            [accountId],
            [clientName],
            [companyName],
            [shipperId],
            [warehouseLocation],
            [updatedBy],
            [totalQuantity],
            [totalWeight],
            [paymentMode],
            [paymentStatus],
            [serviceType],
            [shippingMode],
            [lob],
            [lobStatus],
            [storeId],
            [storeName],
            [channel],
            [referenceId],
            [deliveryTargetDate],
            [deliveryTargetTime],
            [trackingId],
            [internalTrackingId],
            [boxId],
            [boxCount],
            [orderPod],
            [carrier],
            [labelPrinted],
            [notification],
            [addInfoCompanyName],
            [facilityName],
            [beginDate],
            [reason],
            [isAsnGenerated],
            [orderState],
            [otm],
            [carrierAccountId],
            [shipmentType],
            [customerId],
            [isFulfilment],
            [label],
            [labelUrl],
            [isCCP],
            [shipFromName],
            [shipFromAddress],
            [shipFromCity],
            [shipFromState],
            [shipFromCountry],
            [shipFromPostalCode],
            [shipFromEmail],
            [shipFromMobile],
            [shipToName],
            [shipToAddress],
            [shipToAddress2],
            [shipToCity],
            [shipToState],
            [shipToCountry],
            [postalCode],
            [shipToEmail],
            [shipToMobile],
            [billToName],
            [billToAddress],
            [billToAddress2],
            [billToCity],
            [billToState],
            [billToCountry],
            [billToPostal],
            [billToEmail],
            [billToMobile],
            [containerizedOrder],
            [multiItemOrders],
            [shipstationOrder],
            [updateCarrier],
            [rateCardEnable],
            [batchGenerated],
            [orderType],
            [icePack],
            [orderDate],
            [ssPackingSlip],
            [ssShippingLabel],
            [productTypeDesc],
            [carrierLabelUrl],
            [connectorEmailStatus],
            [skuGroup],
            [fulfilment]
        FROM @DataToInsert;
    END
    ELSE
    BEGIN
        PRINT 'Temporary table ##TempTableDataShip does not exist. Creating the table...';

        -- Create the temp table if it doesn't exist
        CREATE TABLE ##TempTableDataShip
        (
            [idKey] INT NOT NULL,
            [id] NVARCHAR(50),
            [createdOn] DATETIME,
            [updatedOn] DATETIME,
            [parentOrder] NVARCHAR(50),
            [orderInfoOrderType] NVARCHAR(50),
            [orderTypeDesc] NVARCHAR(255),
            [status] INT,
            [orderStatusDesc] NVARCHAR(255),
            [accountId] NVARCHAR(50),
            [clientName] NVARCHAR(255),
            [companyName] NVARCHAR(255),
            [shipperId] NVARCHAR(50),
            [warehouseLocation] NVARCHAR(255),
            [updatedBy] NVARCHAR(255),
            [totalQuantity] INT,
            [totalWeight] FLOAT,
            [paymentMode] NVARCHAR(50),
            [paymentStatus] NVARCHAR(50),
            [serviceType] NVARCHAR(50),
            [shippingMode] NVARCHAR(50),
            [lob] NVARCHAR(50),
            [lobStatus] NVARCHAR(50),
            [storeId] NVARCHAR(50),
            [storeName] NVARCHAR(255),
            [channel] NVARCHAR(50),
            [referenceId] NVARCHAR(255),
            [deliveryTargetDate] DATETIME,
            [deliveryTargetTime] NVARCHAR(50),
            [trackingId] NVARCHAR(50),
            [internalTrackingId] NVARCHAR(50),
            [boxId] NVARCHAR(50),
            [boxCount] INT,
            [orderPod] NVARCHAR(255),
            [carrier] NVARCHAR(50),
            [labelPrinted] BIT,
            [notification] NVARCHAR(255),
            [addInfoCompanyName] NVARCHAR(255),
            [facilityName] NVARCHAR(255),
            [beginDate] DATETIME,
            [reason] NVARCHAR(255),
            [isAsnGenerated] BIT,
            [orderState] NVARCHAR(50),
            [otm] NVARCHAR(50),
            [carrierAccountId] NVARCHAR(50),
            [shipmentType] NVARCHAR(50),
            [customerId] NVARCHAR(50),
            [isFulfilment] BIT,
            [label] NVARCHAR(255),
            [labelUrl] NVARCHAR(255),
            [isCCP] BIT,
            [shipFromName] NVARCHAR(255),
            [shipFromAddress] NVARCHAR(255),
            [shipFromCity] NVARCHAR(255),
            [shipFromState] NVARCHAR(50),
            [shipFromCountry] NVARCHAR(50),
            [shipFromPostalCode] NVARCHAR(50),
            [shipFromEmail] NVARCHAR(255),
            [shipFromMobile] NVARCHAR(50),
            [shipToName] NVARCHAR(255),
            [shipToAddress] NVARCHAR(255),
            [shipToAddress2] NVARCHAR(255),
            [shipToCity] NVARCHAR(255),
            [shipToState] NVARCHAR(50),
            [shipToCountry] NVARCHAR(50),
            [postalCode] NVARCHAR(50),
            [shipToEmail] NVARCHAR(255),
            [shipToMobile] NVARCHAR(50),
            [billToName] NVARCHAR(255),
            [billToAddress] NVARCHAR(255),
            [billToAddress2] NVARCHAR(255),
            [billToCity] NVARCHAR(255),
            [billToState] NVARCHAR(50),
            [billToCountry] NVARCHAR(50),
            [billToPostal] NVARCHAR(50),
            [billToEmail] NVARCHAR(255),
            [billToMobile] NVARCHAR(50),
            [containerizedOrder] BIT,
            [multiItemOrders] BIT,
            [shipstationOrder] BIT,
            [updateCarrier] BIT,
            [rateCardEnable] BIT,
            [batchGenerated] BIT,
            [orderType] NVARCHAR(50),
            [icePack] BIT,
            [orderDate] DATETIME,
            [ssPackingSlip] BIT,
            [ssShippingLabel] BIT,
            [productTypeDesc] NVARCHAR(255),
            [carrierLabelUrl] NVARCHAR(255),
            [connectorEmailStatus] NVARCHAR(50),
            [skuGroup] NVARCHAR(255),
            [fulfilment] NVARCHAR(255)
        );

        -- Insert data with idKey starting from 1
        INSERT INTO ##TempTableDataShip 
        (
            [idKey],
            [id],
            [createdOn],
            [updatedOn],
            [parentOrder],
            [orderInfoOrderType],
            [orderTypeDesc],
            [status],
            [orderStatusDesc],
            [accountId],
            [clientName],
            [companyName],
            [shipperId],
            [warehouseLocation],
            [updatedBy],
            [totalQuantity],
            [totalWeight],
            [paymentMode],
            [paymentStatus],
            [serviceType],
            [shippingMode],
            [lob],
            [lobStatus],
            [storeId],
            [storeName],
            [channel],
            [referenceId],
            [deliveryTargetDate],
            [deliveryTargetTime],
            [trackingId],
            [internalTrackingId],
            [boxId],
            [boxCount],
            [orderPod],
            [carrier],
            [labelPrinted],
            [notification],
            [addInfoCompanyName],
            [facilityName],
            [beginDate],
            [reason],
            [isAsnGenerated],
            [orderState],
            [otm],
            [carrierAccountId],
            [shipmentType],
            [customerId],
            [isFulfilment],
            [label],
            [labelUrl],
            [isCCP],
            [shipFromName],
            [shipFromAddress],
            [shipFromCity],
            [shipFromState],
            [shipFromCountry],
            [shipFromPostalCode],
            [shipFromEmail],
            [shipFromMobile],
            [shipToName],
            [shipToAddress],
            [shipToAddress2],
            [shipToCity],
            [shipToState],
            [shipToCountry],
            [postalCode],
            [shipToEmail],
            [shipToMobile],
            [billToName],
            [billToAddress],
            [billToAddress2],
            [billToCity],
            [billToState],
            [billToCountry],
            [billToPostal],
            [billToEmail],
            [billToMobile],
            [containerizedOrder],
            [multiItemOrders],
            [shipstationOrder],
            [updateCarrier],
            [rateCardEnable],
            [batchGenerated],
            [orderType],
            [icePack],
            [orderDate],
            [ssPackingSlip],
            [ssShippingLabel],
            [productTypeDesc],
            [carrierLabelUrl],
            [connectorEmailStatus],
            [skuGroup],
            [fulfilment]
        )
        SELECT 
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [idKey],
            [id],
            [createdOn],
            [updatedOn],
            [parentOrder],
            [orderInfoOrderType],
            [orderTypeDesc],
            [status],
            [orderStatusDesc],
            [accountId],
            [clientName],
            [companyName],
            [shipperId],
            [warehouseLocation],
            [updatedBy],
            [totalQuantity],
            [totalWeight],
            [paymentMode],
            [paymentStatus],
            [serviceType],
            [shippingMode],
            [lob],
            [lobStatus],
            [storeId],
            [storeName],
            [channel],
            [referenceId],
            [deliveryTargetDate],
            [deliveryTargetTime],
            [trackingId],
            [internalTrackingId],
            [boxId],
            [boxCount],
            [orderPod],
            [carrier],
            [labelPrinted],
            [notification],
            [addInfoCompanyName],
            [facilityName],
            [beginDate],
            [reason],
            [isAsnGenerated],
            [orderState],
            [otm],
            [carrierAccountId],
            [shipmentType],
            [customerId],
            [isFulfilment],
            [label],
            [labelUrl],
            [isCCP],
            [shipFromName],
            [shipFromAddress],
            [shipFromCity],
            [shipFromState],
            [shipFromCountry],
            [shipFromPostalCode],
            [shipFromEmail],
            [shipFromMobile],
            [shipToName],
            [shipToAddress],
            [shipToAddress2],
            [shipToCity],
            [shipToState],
            [shipToCountry],
            [postalCode],
            [shipToEmail],
            [shipToMobile],
            [billToName],
            [billToAddress],
            [billToAddress2],
            [billToCity],
            [billToState],
            [billToCountry],
            [billToPostal],
            [billToEmail],
            [billToMobile],
            [containerizedOrder],
            [multiItemOrders],
            [shipstationOrder],
            [updateCarrier],
            [rateCardEnable],
            [batchGenerated],
            [orderType],
            [icePack],
            [orderDate],
            [ssPackingSlip],
            [ssShippingLabel],
            [productTypeDesc],
            [carrierLabelUrl],
            [connectorEmailStatus],
            [skuGroup],
            [fulfilment]
        FROM @DataToInsert;
    END;

    PRINT 'TempTableDataShip popolata con successo';
END;
GO




-----------------------------------------------------------------------------------------------------------------------




USE [yourdb]
GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[WEShipExpress_20241111]
		@jsonBody = N'{"orderNumber": "4716039536792"}'

SELECT	'Return Value' = @return_value

GO

---------------------------------------------------------------------------------------------

------ the most important part
ALTER DATABASE [yourdb] SET TRUSTWORTHY ON;
sp_configure 'clr enabled', 1;
RECONFIGURE;

USE master
GRANT UNSAFE ASSEMBLY to [sample]


DECLARE @strutturaTabella [dbo].[weshipexpresspro_temp_ships];
SELECT *
into ##TempTableDataShip
FROM @strutturaTabella;

DECLARE @strutturaTabellaTransactions [dbo].[weshipexpresspro_temp_shipstransactions];
SELECT *
into  ##TempTableDataShipTransactions
FROM @strutturaTabellaTransactions;




SELECT *
from  ##TempTableDataShip



SELECT *
from   ##TempTableDataShipTransactions


---------------------------------------------

ALTER DATABASE [API_Ship_v1] SET TRUSTWORTHY ON;
sp_configure 'clr enabled', 1;
RECONFIGURE;


