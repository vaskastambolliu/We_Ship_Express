using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using API_We_Ship_Express;
using System.Collections.Generic;
using System.Net.Http;
using System.Globalization;
using System.Transactions;

namespace WEShipExpress_20241111
{
    public partial class StoredProcedures
    {
        [Microsoft.SqlServer.Server.SqlProcedure(Name = "WEShipExpress_20241111")]
        public static async void OrderTrackingProcedure(SqlString jsonBody)
        {
            // Initialize DataTable with specified structure
            DataTable trackingData = new DataTable("trackingData");
            AddTrackingDataColumns(trackingData);

            DataTable trackingTransactionData = new DataTable("trackingTransactionData");
            AddTrackingDataTransactionColumns(trackingTransactionData);

            string jsonBodyString = jsonBody.ToString();

            //string APIUrl = "https://ws-sandbox.advatix.net/weship-fep/api/v1/OrderTracking/getOrderTrackingv2";
            string APIUrl = "https://ws.advatix.net/weship-fep/api/v1/OrderTracking/getOrderTrackingv2";

            try
            {
                // Log initial information
                SqlContext.Pipe.Send("sono arrivati questi dati:");
                SqlContext.Pipe.Send("------------------------------------------");
                SqlContext.Pipe.Send("jsonBody");
                SqlContext.Pipe.Send(jsonBodyString);
                SqlContext.Pipe.Send("------------------------------------------");


                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls13;
                WebPermission permission = new WebPermission(NetworkAccess.Connect, APIUrl);
                permission.AddPermission(NetworkAccess.Connect, APIUrl);
                permission.AddPermission(NetworkAccess.Accept, APIUrl);
                permission.Demand();

                IEnumerator myConnectEnum = permission.ConnectList;

                while (myConnectEnum.MoveNext())
                { }

                IEnumerator myAcceptEnum = permission.AcceptList;

                // Configure HTTP request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrl);
                string requestBody = jsonBodyString;
                SqlContext.Pipe.Send("Popolo HTTP request");
                byte[] bytes;
                bytes = Encoding.ASCII.GetBytes(requestBody);


                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";
                request.Headers.Add("APIKey", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJcdTAwMTFcdTAwMUYtOjlcdTAwMTZcXDZIXCImPlx1MDAwRnxcXFx1MDAxMigtXHUwMDE2XHUwMDE0XHUwMDE5XHUwMDA0XGZsXVVFRVx1MDAxNlx1MDAwNVx1MDAwRVtmXmsiLCJpc3MiOiJjb20uYWR2YXRpeC5zZXJ2aWNlcyIsImV4cCI6MTY1NjMzNjQ3MCwiaWF0IjoxNjU2MzI5MjcwLCJqdGk");
                request.Headers.Add("AccountId", "BTV");
                request.Headers.Add("DEVICE-TYPE", "Web");
                request.Headers.Add("VER", "1.0");

                //SetRequestHeaders(request);
                SqlContext.Pipe.Send("request: " + request);

                // Prepare request body
                //string jsonBodyString = "{\"orderNumber\": \"4716039536792\"}";  // replace with actual jsonBody content if needed
                //SqlContext.Pipe.Send("Preparo Stream per chiamata");
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();

                SqlContext.Pipe.Send("requestBody: " + requestBody);
                // Process HTTP response
                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    SqlContext.Pipe.Send("StatusCode: " + response.StatusCode);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        SqlContext.Pipe.Send("StatusCode: " + response.StatusCode);
                        //HandleSuccessfulResponse(response, trackingData);


                        Stream responseStream = null;
                        string responseStr = "";
                        try
                        {
                            responseStream = response.GetResponseStream();
                            responseStr = new StreamReader(responseStream).ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            SqlContext.Pipe.Send("responseStream ex: " + ex);
                            HandleGeneralException(ex);
                            throw;
                        }

                        // Parse JSON response and populate trackingData
                        //var jsonObject = new JObject();
                        var jsonObject = JObject.Parse(responseStr);
                        //SendLongMessage("jsonObject: " + jsonObject);
                        try
                        {

                            JToken labelUrlToken;
                            if (jsonObject.TryGetValue("labelUrl", out labelUrlToken))
                            {
                                if (labelUrlToken.Type == JTokenType.String)
                                {
                                    var urlString = labelUrlToken.ToString();
                                    if (!string.IsNullOrEmpty(urlString))
                                    {
                                        jsonObject["labelUrl"] = new JArray(urlString);
                                    }
                                    else
                                    {
                                        jsonObject["labelUrl"] = new JArray(); // Empty string case
                                    }
                                }
                                else if (labelUrlToken.Type == JTokenType.Array)
                                {
                                    // If it's an array, do nothing, it's already correct
                                }
                                else
                                {
                                    jsonObject["labelUrl"] = new JArray(); // Handle empty or other cases
                                }
                            }
                            else
                            {
                                jsonObject["labelUrl"] = new JArray(); // Handle case where LabelURL is missing
                            }

                            // Convert back to JSON string
                            responseStr = jsonObject.ToString(Formatting.Indented);


                            // Deserialize the JSON into a dynamic object
                            var responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseStr);
                            //SendLongMessage("responseObject: " + responseObject);




                            // Create a new DataRow
                            DataRow row = trackingData.NewRow();


                            // Populate the DataRow with the deserialized data
                            row["Id"] = responseObject.responseObject.orderNumber.id;
                            SendLongMessage("Id: " + responseObject.responseObject.orderNumber.id);

                            row["CreatedOn"] = responseObject.responseObject.orderInfo.createdOn;
                            SendLongMessage("CreatedOn: " + responseObject.responseObject.orderInfo.createdOn);

                            row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            SendLongMessage("UpdatedOn: " + responseObject.responseObject.orderInfo.updatedOn);
                            row["ParentOrder"] = responseObject.responseObject.orderInfo.parentOrder;
                            row["OrderType"] = responseObject.responseObject.orderInfo.orderType;
                            row["OrderTypeDesc"] = responseObject.responseObject.orderInfo.orderTypeDesc;
                            row["Status"] = responseObject.responseObject.orderInfo.status;
                            row["OrderStatusDesc"] = responseObject.responseObject.orderInfo.orderStatusDesc;
                            row["AccountId"] = responseObject.responseObject.orderInfo.accountId;
                            row["ClientName"] = responseObject.responseObject.orderInfo.clientName;
                            row["CompanyName"] = responseObject.responseObject.orderInfo.companyName;
                            row["ShipperId"] = responseObject.responseObject.orderInfo.shipperId;
                            row["WarehouseLocation"] = responseObject.responseObject.orderInfo.warehouseLocation;
                            row["UpdatedBy"] = responseObject.responseObject.orderInfo.updatedBy;
                            row["TotalQuantity"] = responseObject.responseObject.orderInfo.totalQuantity;
                            row["TotalWeight"] = responseObject.responseObject.orderInfo.totalWeight;
                            row["paymentMode"] = responseObject.responseObject.orderInfo.paymentMode;
                            row["PaymentStatus"] = responseObject.responseObject.orderInfo.paymentStatus;
                            row["ServiceType"] = responseObject.responseObject.orderInfo.serviceType;
                            row["ShippingMode"] = responseObject.responseObject.orderInfo.shippingMode;
                            row["Lob"] = responseObject.responseObject.orderInfo.lob;
                            row["StoreId"] = responseObject.responseObject.orderInfo.storeId;
                            row["Channel"] = responseObject.responseObject.additionalInfo.channel;
                            row["ReferenceId"] = responseObject.responseObject.additionalInfo.referenceId;
                            row["DeliveryTargetDate"] = responseObject.responseObject.additionalInfo.deliveryTargetDate;
                            row["DeliveryTargetTime"] = responseObject.responseObject.additionalInfo.deliveryTargetTime;
                            row["TrackingId"] = responseObject.responseObject.additionalInfo.trackingId;
                            row["InternalTrackingId"] = responseObject.responseObject.additionalInfo.internalTrackingId;
                            row["BoxId"] = responseObject.responseObject.additionalInfo.boxId;
                            row["BoxCount"] = responseObject.responseObject.additionalInfo.boxCount;
                            row["OrderPod"] = responseObject.responseObject.additionalInfo.orderPod;
                            row["Carrier"] = responseObject.responseObject.additionalInfo.carrier;
                            row["LabelPrinted"] = responseObject.responseObject.additionalInfo.labelPrinted;
                            row["Notification"] = responseObject.responseObject.additionalInfo.notification;
                            row["CompanyName"] = responseObject.responseObject.additionalInfo.companyName;
                            row["FacilityName"] = responseObject.responseObject.additionalInfo.facilityName;
                            row["BeginDate"] = responseObject.responseObject.additionalInfo.beginDate;
                            row["Reason"] = responseObject.responseObject.additionalInfo.reason;
                            row["IsAsnGenerated"] = responseObject.responseObject.additionalInfo.isAsnGenerated;
                            row["OrderState"] = responseObject.responseObject.additionalInfo.orderState;
                            row["Otm"] = responseObject.responseObject.additionalInfo.otm;
                            row["CarrierAccountId"] = responseObject.responseObject.additionalInfo.carrierAccountId;
                            row["ShipmentType"] = responseObject.responseObject.additionalInfo.shipmentType;
                            row["CustomerId"] = responseObject.responseObject.additionalInfo.customerId;
                            row["IsFulfilment"] = responseObject.responseObject.additionalInfo.isFulfilment;
                            row["Label"] = responseObject.responseObject.additionalInfo.label;

                            //TO CONVERT IN BASE64
                            row["LabelUrl"] = responseObject.responseObject.additionalInfo.labelUrl;

                            //if (!string.IsNullOrEmpty(responseObject.responseObject.additionalInfo.labelUrl.ToString()))
                            //{
                            //    using (var httpClient = new HttpClient())
                            //    {
                            //        // Download the file
                            //        HttpResponseMessage responseurl = await httpClient.GetAsync(responseObject.responseObject.additionalInfo.labelUrl.ToString());
                            //        // Step 1: Download the file
                            //        //var responseurl = await httpClient.GetAsync(responseObject.responseObject.additionalInfo.labelUrl.ToString());
                            //        if (responseurl.IsSuccessStatusCode)
                            //        {
                            //            // Convert file to byte array and then Base64
                            //            byte[] fileBytes = await responseurl.Content.ReadAsByteArrayAsync();
                            //            string base64File = Convert.ToBase64String(fileBytes);

                            //            row["LabelUrl"] = base64File;
                            //            SendLongMessage("responseurl: " + base64File);

                            //        }
                            //        else
                            //        {
                            //            SendLongMessage("responseurl: Failed to download file from URL.");
                            //            throw new Exception("Failed to download file from URL.");
                            //        }
                            //    }
                            //}



                            row["IsCCP"] = responseObject.responseObject.additionalInfo.isCCP;

                            row["ShipFromName"] = responseObject.responseObject.shipFromInfo.shipFromName;
                            row["ShipFromAddress"] = responseObject.responseObject.shipFromInfo.shipFromAddress;
                            row["ShipFromCity"] = responseObject.responseObject.shipFromInfo.shipFromCity;
                            row["ShipFromState"] = responseObject.responseObject.shipFromInfo.shipFromState;
                            row["ShipFromCountry"] = responseObject.responseObject.shipFromInfo.shipFromCountry;
                            row["ShipFromPostalCode"] = responseObject.responseObject.shipFromInfo.shipFromPostalCode;
                            row["ShipFromEmail"] = responseObject.responseObject.shipFromInfo.shipFromEmail;
                            row["ShipFromMobile"] = responseObject.responseObject.shipFromInfo.shipFromMobile;

                            row["ShipToName"] = responseObject.responseObject.shipToInfo.shipToName;
                            row["ShipToAddress"] = responseObject.responseObject.shipToInfo.shipToAddress;
                            row["ShipToAddress2"] = responseObject.responseObject.shipToInfo.shipToAddress2;
                            row["ShipToCity"] = responseObject.responseObject.shipToInfo.shipToCity;
                            row["ShipToState"] = responseObject.responseObject.shipToInfo.shipToState;
                            row["ShipToCountry"] = responseObject.responseObject.shipToInfo.shipToCountry;
                            row["PostalCode"] = responseObject.responseObject.shipToInfo.postalCode;
                            row["ShipToEmail"] = responseObject.responseObject.shipToInfo.shipToEmail;
                            row["ShipToMobile"] = responseObject.responseObject.shipToInfo.shipToMobile;

                            row["BillToName"] = responseObject.responseObject.billToInfo.billToName;
                            row["BillToAddress"] = responseObject.responseObject.billToInfo.billToAddress;
                            row["BillToAddress2"] = responseObject.responseObject.billToInfo.billToAddress2;
                            row["BillToCity"] = responseObject.responseObject.billToInfo.billToCity;
                            row["BillToState"] = responseObject.responseObject.billToInfo.billToState;
                            row["BillToCountry"] = responseObject.responseObject.billToInfo.billToCountry;
                            row["BillToPostal"] = responseObject.responseObject.billToInfo.billToPostal;
                            row["BillToEmail"] = responseObject.responseObject.billToInfo.billToEmail;
                            row["BillToMobile"] = responseObject.responseObject.billToInfo.billToMobile;



                            row["ContainerizedOrder"] = responseObject.responseObject.containerizedOrder;
                            row["MultiItemOrders"] = responseObject.responseObject.multiItemOrders;
                            row["ShipstationOrder"] = responseObject.responseObject.shipstationOrder;


                            row["UpdateCarrier"] = responseObject.responseObject.updateCarrier;
                            row["RateCardEnable"] = responseObject.responseObject.rateCardEnable;
                            row["BatchGenerated"] = responseObject.responseObject.batchGenerated;
                            row["OrderType"] = responseObject.responseObject.orderInfo.orderType;
                            row["IcePack"] = responseObject.responseObject.icePack;
                            row["OrderDate"] = responseObject.responseObject.orderDate;
                            row["SsPackingSlip"] = responseObject.responseObject.ssPackingSlip;
                            row["SsShippingLabel"] = responseObject.responseObject.ssShippingLabel;
                            row["ProductTypeDesc"] = responseObject.responseObject.productTypeDesc;
                            row["CarrierLabelUrl"] = responseObject.responseObject.carrierLabelUrl;
                            row["ConnectorEmailStatus"] = responseObject.responseObject.connectorEmailStatus;
                            row["SkuGroup"] = responseObject.responseObject.skuGroup;
                            row["Fulfilment"] = responseObject.responseObject.fulfilment;


                            // Add the DataRow to the DataTable
                            trackingData.Rows.Add(row);

                            //salvatagio dei dato del ordertracking

                            if (responseObject.responseObject.orderTransactions != null && responseObject.responseObject.orderTransactions.Count > 0)
                            {

                                // Parse the orderTransactions array
                                JArray orderTransactions = (JArray)jsonObject["responseObject"]["orderTransactions"];
                                //SendLongMessage("orderTransactions: " + orderTransactions);

                                foreach (JObject transaction in orderTransactions)
                                {

                                    // Create a new DataRow for orderTransaction
                                    DataRow rowTransaction = trackingTransactionData.NewRow();

                                    rowTransaction["Status"] = (int)transaction["status"];
                                    rowTransaction["CreatedOn"] = DateTimeOffset.ParseExact((string)transaction["createdOn"], "MM-dd-yyyy HH:mm:ss fff zzz", CultureInfo.InvariantCulture).DateTime;
                                    rowTransaction["UpdatedOn"] = DateTimeOffset.ParseExact((string)transaction["updatedOn"], "MM-dd-yyyy HH:mm:ss fff zzz", CultureInfo.InvariantCulture).DateTime;
                                    rowTransaction["Reason"] = (string)transaction["reason"];
                                    rowTransaction["OrderStatusDesc"] = (string)transaction["orderStatusDesc"];
                                    rowTransaction["StatusSequence"] = (int)transaction["statusSequence"];
                                    rowTransaction["ShipToCity"] = (string)transaction["shipToCity"];
                                    rowTransaction["ShipToState"] = (string)transaction["shipToState"];


                                    // Add the rowTransaction to trackingTransactionData
                                    trackingTransactionData.Rows.Add(rowTransaction);

                                }

                            }




                        }
                        catch (Exception ex)
                        {
                            SendLongMessage("jsonObject ex: " + ex);
                            HandleGeneralException(ex);
                            throw;
                        }

                    }
                    else
                    {
                        HandleErrorResponse(response);
                    }

                }
                catch (WebException ex)
                {
                    HandleWebException(ex);
                }
                finally
                {
                    response?.Close();
                }

                try
                {
                    // Ensure the response is closed before continuing.
                    response.Close();

                    // Define connection string
                    string connString = "Data Source=DESKTOP-FO7B6CB\\SQLEXPRESS01;Initial Catalog=API_Ship_v1;User ID=sample;Password=sample";


                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                               new TransactionOptions
                               {
                                   IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, // Adjust based on your requirements
                                   Timeout = TimeSpan.FromMinutes(5) // Set appropriate timeout
                               }))
                    {
                        using (SqlConnection connection = new SqlConnection(connString))
                        {
                            connection.Open();

                            // Insert into weshipexpresspro_temp_ships
                            using (SqlCommand cmd = new SqlCommand("dbo.WEShipExpress_importData_20241111", connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Define and add the parameter
                                SqlParameter param = new SqlParameter
                                {
                                    ParameterName = "@DataToInsert",
                                    SqlDbType = SqlDbType.Structured,
                                    Value = trackingData,
                                    TypeName = "dbo.weshipexpresspro_temp_ships"
                                };
                                cmd.Parameters.Add(param);

                                SendLongMessage("param: " + param);
                                cmd.ExecuteNonQuery();
                            }

                            SendLongMessage("InsertDataToDatabase in weshipexpresspro_temp_ships: " + response.StatusCode);

                            // Insert into weshipexpresspro_temp_shipstransactions
                            using (SqlCommand cmd = new SqlCommand("dbo.WEShipExpressTransactions_importData_20241111", connection))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;

                                // Define and add the parameter
                                SqlParameter param = new SqlParameter
                                {
                                    ParameterName = "@DataToInsertTransaction",
                                    SqlDbType = SqlDbType.Structured,
                                    Value = trackingTransactionData,
                                    TypeName = "dbo.weshipexpresspro_temp_shipstransactions"
                                };
                                cmd.Parameters.Add(param);

                                SendLongMessage("param: " + param);
                                cmd.ExecuteNonQuery();
                            }

                            SendLongMessage("InsertDataToDatabase in weshipexpresspro_temp_shipstransactions: " + response.StatusCode);
                        }

                        // Complete the transaction
                        scope.Complete();
                    }

                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur
                    SendLongMessage("Error: " + ex.Message);
                    //SqlContext.Pipe.Send("Error: " + ex.Message);
                }
                finally
                {
                    // Ensure proper cleanup (if response is not yet closed)
                    if (response != null)
                    {
                        response.Close();
                    }
                }



                //InsertDataToDatabase(trackingData);
            }
            catch (Exception ex)
            {
                HandleGeneralException(ex);
            }
        }

        private static void AddTrackingDataColumns(DataTable orderDetailsTable)
        {
            // Define DataTable columns based on JSON structure
            // Adding columns with specified properties


            // Define the "IdKey" column as an auto-incrementing integer
            DataColumn idKeyColumn = new DataColumn("IdKey", typeof(int))
            {
                AutoIncrement = true,
                AutoIncrementSeed = 1, // Starting value for the identity
                AutoIncrementStep = 1  // Increment step
            };
            // Add the column to the DataTable
            orderDetailsTable.Columns.Add(idKeyColumn);

            orderDetailsTable.Columns.Add("Id", typeof(string));
            orderDetailsTable.Columns.Add("CreatedOn", typeof(long));
            orderDetailsTable.Columns.Add("UpdatedOn", typeof(long));
            orderDetailsTable.Columns.Add("ParentOrder", typeof(string));
            orderDetailsTable.Columns.Add("OrderInfoOrderType", typeof(int));
            orderDetailsTable.Columns.Add("OrderTypeDesc", typeof(string));
            orderDetailsTable.Columns.Add("Status", typeof(int));
            orderDetailsTable.Columns.Add("OrderStatusDesc", typeof(string));
            orderDetailsTable.Columns.Add("AccountId", typeof(string));
            orderDetailsTable.Columns.Add("ClientName", typeof(string));
            orderDetailsTable.Columns.Add("CompanyName", typeof(string));
            orderDetailsTable.Columns.Add("ShipperId", typeof(int));
            orderDetailsTable.Columns.Add("WarehouseLocation", typeof(string));
            orderDetailsTable.Columns.Add("UpdatedBy", typeof(int));
            orderDetailsTable.Columns.Add("TotalQuantity", typeof(int));
            orderDetailsTable.Columns.Add("TotalWeight", typeof(double));
            orderDetailsTable.Columns.Add("PaymentMode", typeof(int));
            orderDetailsTable.Columns.Add("PaymentStatus", typeof(int));
            orderDetailsTable.Columns.Add("ServiceType", typeof(string));
            orderDetailsTable.Columns.Add("ShippingMode", typeof(string));
            orderDetailsTable.Columns.Add("Lob", typeof(int));
            orderDetailsTable.Columns.Add("LobStatus", typeof(string));
            orderDetailsTable.Columns.Add("StoreId", typeof(string));
            orderDetailsTable.Columns.Add("StoreName", typeof(string));
            orderDetailsTable.Columns.Add("Channel", typeof(string));
            orderDetailsTable.Columns.Add("ReferenceId", typeof(string));
            orderDetailsTable.Columns.Add("DeliveryTargetDate", typeof(long));
            orderDetailsTable.Columns.Add("DeliveryTargetTime", typeof(string));
            orderDetailsTable.Columns.Add("TrackingId", typeof(string));
            orderDetailsTable.Columns.Add("InternalTrackingId", typeof(string));
            orderDetailsTable.Columns.Add("BoxId", typeof(string));
            orderDetailsTable.Columns.Add("BoxCount", typeof(int));
            orderDetailsTable.Columns.Add("OrderPod", typeof(string));
            orderDetailsTable.Columns.Add("Carrier", typeof(string));
            orderDetailsTable.Columns.Add("LabelPrinted", typeof(int));
            orderDetailsTable.Columns.Add("Notification", typeof(bool));
            orderDetailsTable.Columns.Add("AddInfoCompanyName", typeof(string));
            orderDetailsTable.Columns.Add("FacilityName", typeof(string));
            orderDetailsTable.Columns.Add("BeginDate", typeof(string));
            orderDetailsTable.Columns.Add("Reason", typeof(string));
            orderDetailsTable.Columns.Add("IsAsnGenerated", typeof(bool));
            orderDetailsTable.Columns.Add("OrderState", typeof(int));
            orderDetailsTable.Columns.Add("Otm", typeof(int));
            orderDetailsTable.Columns.Add("CarrierAccountId", typeof(string));
            orderDetailsTable.Columns.Add("ShipmentType", typeof(string));
            orderDetailsTable.Columns.Add("CustomerId", typeof(string));
            orderDetailsTable.Columns.Add("IsFulfilment", typeof(int));
            orderDetailsTable.Columns.Add("Label", typeof(int));
            orderDetailsTable.Columns.Add("LabelUrl", typeof(string));
            orderDetailsTable.Columns.Add("IsCCP", typeof(bool));
            orderDetailsTable.Columns.Add("ShipFromName", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromAddress", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromCity", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromState", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromCountry", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromPostalCode", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromEmail", typeof(string));
            orderDetailsTable.Columns.Add("ShipFromMobile", typeof(string));
            orderDetailsTable.Columns.Add("ShipToName", typeof(string));
            orderDetailsTable.Columns.Add("ShipToAddress", typeof(string));
            orderDetailsTable.Columns.Add("ShipToAddress2", typeof(string));
            orderDetailsTable.Columns.Add("ShipToCity", typeof(string));
            orderDetailsTable.Columns.Add("ShipToState", typeof(string));
            orderDetailsTable.Columns.Add("ShipToCountry", typeof(string));
            orderDetailsTable.Columns.Add("PostalCode", typeof(string));
            orderDetailsTable.Columns.Add("ShipToEmail", typeof(string));
            orderDetailsTable.Columns.Add("ShipToMobile", typeof(string));
            orderDetailsTable.Columns.Add("BillToName", typeof(string));
            orderDetailsTable.Columns.Add("BillToAddress", typeof(string));
            orderDetailsTable.Columns.Add("BillToAddress2", typeof(string));
            orderDetailsTable.Columns.Add("BillToCity", typeof(string));
            orderDetailsTable.Columns.Add("BillToState", typeof(string));
            orderDetailsTable.Columns.Add("BillToCountry", typeof(string));
            orderDetailsTable.Columns.Add("BillToPostal", typeof(string));
            orderDetailsTable.Columns.Add("BillToEmail", typeof(string));
            orderDetailsTable.Columns.Add("BillToMobile", typeof(string));
            orderDetailsTable.Columns.Add("ContainerizedOrder", typeof(bool));
            orderDetailsTable.Columns.Add("MultiItemOrders", typeof(int));
            orderDetailsTable.Columns.Add("ShipstationOrder", typeof(int));
            orderDetailsTable.Columns.Add("UpdateCarrier", typeof(bool));
            orderDetailsTable.Columns.Add("RateCardEnable", typeof(bool));
            orderDetailsTable.Columns.Add("BatchGenerated", typeof(bool));
            orderDetailsTable.Columns.Add("OrderType", typeof(string));
            orderDetailsTable.Columns.Add("IcePack", typeof(int));
            orderDetailsTable.Columns.Add("OrderDate", typeof(long));
            orderDetailsTable.Columns.Add("SsPackingSlip", typeof(int));
            orderDetailsTable.Columns.Add("SsShippingLabel", typeof(int));
            orderDetailsTable.Columns.Add("ProductTypeDesc", typeof(string));
            orderDetailsTable.Columns.Add("CarrierLabelUrl", typeof(string));
            orderDetailsTable.Columns.Add("ConnectorEmailStatus", typeof(int));
            orderDetailsTable.Columns.Add("SkuGroup", typeof(string));
            orderDetailsTable.Columns.Add("Fulfilment", typeof(string));
        }

        private static void AddTrackingDataTransactionColumns(DataTable orderDetailsTransactionTable)
        {
            // Define DataTable columns based on JSON structure
            // Adding columns with specified properties
            // Define the "IdKey" column as an auto-incrementing integer
            DataColumn idKeyColumn = new DataColumn("IdKey", typeof(int))
            {
                AutoIncrement = true,
                AutoIncrementSeed = 1, // Starting value for the identity
                AutoIncrementStep = 1  // Increment step
            };
            // Add the column to the DataTable
            orderDetailsTransactionTable.Columns.Add(idKeyColumn);

            //orderDetailsTransactionTable.Columns.Add("IdKey", typeof(int));

            orderDetailsTransactionTable.Columns.Add("OrderNumber", typeof(string));
            orderDetailsTransactionTable.Columns.Add("Status", typeof(string));
            orderDetailsTransactionTable.Columns.Add("CreatedOn", typeof(DateTime));
            orderDetailsTransactionTable.Columns.Add("UpdatedOn", typeof(DateTime));
            orderDetailsTransactionTable.Columns.Add("Reason", typeof(string));
            orderDetailsTransactionTable.Columns.Add("OrderStatusDesc", typeof(string));
            orderDetailsTransactionTable.Columns.Add("StatusSequence", typeof(int));
            orderDetailsTransactionTable.Columns.Add("ShipToCity", typeof(string));
            orderDetailsTransactionTable.Columns.Add("ShipToState", typeof(string));
        }

        private static async Task HandleErrorResponse(HttpWebResponse response)
        {
            SqlContext.Pipe.Send("Error: Response status code is " + response.StatusCode);
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string errorContent = await reader.ReadToEndAsync();
                SqlContext.Pipe.Send("Error details: " + errorContent);
            }
        }

        private static async Task HandleWebException(WebException ex)
        {
            if (ex.Response is HttpWebResponse errorResponse)
            {
                SqlContext.Pipe.Send("Exception: " + errorResponse.StatusCode);
                using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream()))
                {
                    string errorContent = await reader.ReadToEndAsync();
                    SqlContext.Pipe.Send("Error response content: " + errorContent);
                }
            }
            else
            {
                SqlContext.Pipe.Send("An unexpected error occurred: " + ex.Message);
            }
        }


        private static async Task HandleGeneralException(Exception ex)
        {
            await SendLongMessage("Handled exception: " + ex.Message);
        }


        // Helper method to send messages in chunks
        private static async Task SendLongMessage(string message)
        {
            const int maxChunkSize = 4000;
            int messageLength = message.Length;
            for (int i = 0; i < messageLength; i += maxChunkSize)
            {
                string chunk = message.Substring(i, Math.Min(maxChunkSize, messageLength - i));
                SqlContext.Pipe.Send(chunk);
            }
        }

    }
}
