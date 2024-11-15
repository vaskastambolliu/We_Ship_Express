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

            string jsonBodyString = jsonBody.ToString();

            string APIUrl = "https://ws-sandbox.advatix.net/weship-fep/api/v1/OrderTracking/getOrderTrackingv2";

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
                        SendLongMessage("jsonObject: " + jsonObject);
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
                            SendLongMessage("responseObject: " + responseObject);




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

                            if (!string.IsNullOrEmpty(responseObject.responseObject.additionalInfo.labelUrl.ToString()))
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    // Download the file
                                    HttpResponseMessage responseurl = await httpClient.GetAsync(responseObject.responseObject.additionalInfo.labelUrl.ToString());
                                    // Step 1: Download the file
                                    //var responseurl = await httpClient.GetAsync(responseObject.responseObject.additionalInfo.labelUrl.ToString());
                                    if (responseurl.IsSuccessStatusCode)
                                    {
                                        // Convert file to byte array and then Base64
                                        byte[] fileBytes = await responseurl.Content.ReadAsByteArrayAsync();
                                        string base64File = Convert.ToBase64String(fileBytes);

                                        row["LabelUrl"] = base64File;
                                        SendLongMessage("responseurl: " + base64File);

                                    }
                                    else
                                    {
                                        throw new Exception("Failed to download file from URL.");
                                        SendLongMessage("responseurl: Failed to download file from URL.");
                                    }
                                }
                            }



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

                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;
                            //row["UpdatedOn"] = responseObject.responseObject.orderInfo.updatedOn;


                            // (add all other field mappings here)

                            // Add the DataRow to the DataTable
                            trackingData.Rows.Add(row);

                        }
                        catch (Exception ex)
                        {

                            SqlContext.Pipe.Send("jsonObject ex: " + ex);
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

                    // Create and open the SQL connection
                    using (SqlConnection connection = new SqlConnection(connString))
                    {
                        connection.Open();

                        // Create a SQL command to execute the stored procedure
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "dbo.WEShipExpress_importData_20241111";
                            cmd.Connection = connection;

                            // Define the parameter for the stored procedure
                            SqlParameter param = new SqlParameter
                            {
                                ParameterName = "@DataToInsert",
                                SqlDbType = SqlDbType.Structured,
                                Value = trackingData,
                                TypeName = "dbo.weshipexpresspro_temp_ships"
                            };
                            cmd.Parameters.Add(param);

                            SendLongMessage("param: " + param);
                            // Execute the stored procedure
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Send the response status to SQL context
                    SqlContext.Pipe.Send("InsertDataToDatabase: " + response.StatusCode);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur
                    SqlContext.Pipe.Send("Error: " + ex.Message);
                }
                finally
                {
                    // Ensure proper cleanup (if response is not yet closed)
                    if (response != null )
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

        private static void SetRequestHeaders(HttpWebRequest request)
        {
            request.Headers.Add("APIKey", "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJcdTAwMTFcdTAwMUYtOjlcdTAwMTZcXDZIXCImPlx1MDAwRnxcXFx1MDAxMigtXHUwMDE2XHUwMDE0XHUwMDE5XHUwMDA0XGZsXVVFRVx1MDAxNlx1MDAwNVx1MDAwRVtmXmsiLCJpc3MiOiJjb20uYWR2YXRpeC5zZXJ2aWNlcyIsImV4cCI6MTY1NjMzNjQ3MCwiaWF0IjoxNjU2MzI5MjcwLCJqdGk");
            request.Headers.Add("AccountId", "BTV");
            request.Headers.Add("DEVICE-TYPE", "Web");
            request.Headers.Add("VER", "1.0");
        }

        private static async void HandleSuccessfulResponse(HttpWebResponse response, DataTable trackingData)
        {
            //using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            //{
            Stream responseStream = response.GetResponseStream();
            string responseStr = new StreamReader(responseStream).ReadToEnd();


            // Parse JSON response and populate trackingData
            var jsonObject = JObject.Parse(responseStr);
            DataRow row = trackingData.NewRow();


            var responseStatusToken = jsonObject["responseStatus"];
            if (responseStatusToken != null)
            {
                try
                {
                    // Use Convert.ToBoolean to safely parse true/false values or "true"/"false" strings
                    row["ResponseStatus"] = Convert.ToBoolean(responseStatusToken);
                }
                catch
                {
                    // Default to false if conversion fails
                    row["ResponseStatus"] = false;
                }
            }
            else
            {
                row["ResponseStatus"] = false; // default value if null
            }

            var responseStatusCodeToken = jsonObject["responseStatusCode"];
            if (responseStatusCodeToken != null)
            {
                try
                {
                    // Convert responseStatusCode to an integer safely
                    row["ResponseStatusCode"] = Convert.ToInt32(responseStatusCodeToken);
                }
                catch
                {
                    // Default to 0 if conversion fails
                    row["ResponseStatusCode"] = 0;
                }
            }
            else
            {
                row["ResponseStatusCode"] = 0; // default value if null
            }

            //var responseObject = jsonObject["responseObject"];
            //var orderInfo = responseObject?["orderInfo"];
            //var additionalInfo = responseObject?["additionalInfo"];
            //row["CreatedOn"] = orderInfo?["createdOn"] != null ? Convert.ToInt64(orderInfo["createdOn"]) : 0;
            //row["UpdatedOn"] = orderInfo?["updatedOn"] != null ? Convert.ToInt64(orderInfo["updatedOn"]) : 0;
            //row["OrderType"] = orderInfo?["orderType"] != null ? Convert.ToInt32(orderInfo["orderType"]) : 0;
            //row["Status"] = orderInfo?["status"] != null ? Convert.ToInt32(orderInfo["status"]) : 0;
            //row["ShipperId"] = orderInfo?["shipperId"] != null ? Convert.ToInt32(orderInfo["shipperId"]) : 0;
            //row["UpdatedBy"] = orderInfo?["updatedBy"] != null ? Convert.ToInt32(orderInfo["updatedBy"]) : 0;
            //row["TotalQuantity"] = orderInfo?["totalQuantity"] != null ? Convert.ToInt32(orderInfo["totalQuantity"]) : 0;
            //row["TotalWeight"] = orderInfo?["totalWeight"] != null ? Convert.ToDouble(orderInfo["totalWeight"]) : 0.0;
            //row["PaymentMode"] = orderInfo?["paymentMode"] != null ? Convert.ToInt32(orderInfo["paymentMode"]) : 0;
            //row["PaymentStatus"] = orderInfo?["paymentStatus"] != null ? Convert.ToInt32(orderInfo["paymentStatus"]) : 0;
            //row["Lob"] = orderInfo?["lob"] != null ? Convert.ToInt32(orderInfo["lob"]) : 0;

            //row["DeliveryTargetDate"] = additionalInfo?["deliveryTargetDate"] != null ? Convert.ToInt64(additionalInfo["deliveryTargetDate"]) : 0;
            //row["BoxCount"] = additionalInfo?["boxCount"] != null ? Convert.ToInt32(additionalInfo["boxCount"]) : 0;
            //row["LabelPrinted"] = additionalInfo?["labelPrinted"] != null ? Convert.ToInt32(additionalInfo["labelPrinted"]) : 0;
            //row["Notification"] = additionalInfo?["notification"] != null ? Convert.ToBoolean(additionalInfo["notification"]) : false;
            //row["IsAsnGenerated"] = additionalInfo?["isAsnGenerated"] != null ? Convert.ToBoolean(additionalInfo["isAsnGenerated"]) : false;
            //row["OrderState"] = additionalInfo?["orderState"] != null ? Convert.ToInt32(additionalInfo["orderState"]) : 0;
            //row["Otm"] = additionalInfo?["otm"] != null ? Convert.ToInt32(additionalInfo["otm"]) : 0;
            //row["IsFulfilment"] = additionalInfo?["isFulfilment"] != null ? Convert.ToInt32(additionalInfo["isFulfilment"]) : 0;
            //row["Label"] = additionalInfo?["label"] != null ? Convert.ToInt32(additionalInfo["label"]) : 0;
            //row["IsCcp"] = additionalInfo?["isCCP"] != null ? Convert.ToBoolean(additionalInfo["isCCP"]) : false;
            //// Order Number
            //row["OrderNumber"] = (string)jsonObject["responseObject"]?["orderNumber"]?["id"];

            //// Order Info

            //row["ParentOrder"] = (string)jsonObject["responseObject"]?["orderInfo"]?["parentOrder"];

            //row["OrderTypeDesc"] = (string)jsonObject["responseObject"]?["orderInfo"]?["orderTypeDesc"];

            //row["OrderStatusDesc"] = (string)jsonObject["responseObject"]?["orderInfo"]?["orderStatusDesc"];
            //row["AccountId"] = (string)jsonObject["responseObject"]?["orderInfo"]?["accountId"];
            //row["ClientName"] = (string)jsonObject["responseObject"]?["orderInfo"]?["clientName"];
            //row["CompanyName"] = (string)jsonObject["responseObject"]?["orderInfo"]?["companyName"];

            //row["WarehouseLocation"] = (string)jsonObject["responseObject"]?["orderInfo"]?["warehouseLocation"];

            //row["ServiceType"] = (string)jsonObject["responseObject"]?["orderInfo"]?["serviceType"];
            //row["ShippingMode"] = (string)jsonObject["responseObject"]?["orderInfo"]?["shippingMode"];

            //row["LobStatus"] = (string)jsonObject["responseObject"]?["orderInfo"]?["lobStatus"];
            //row["StoreId"] = (string)jsonObject["responseObject"]?["orderInfo"]?["storeId"];
            //row["StoreName"] = (string)jsonObject["responseObject"]?["orderInfo"]?["storeName"];

            //// Additional Info
            //row["Channel"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["channel"];
            //row["ReferenceId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["referenceId"];

            //row["DeliveryTargetTime"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["deliveryTargetTime"];
            //row["TrackingId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["trackingId"];
            //row["InternalTrackingId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["internalTrackingId"];
            //row["BoxId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["boxId"];

            //row["OrderPod"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["orderPod"];
            //row["Carrier"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["carrier"];

            //row["CompanyName"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["companyName"];
            //row["FacilityName"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["facilityName"];
            //row["BeginDate"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["beginDate"];
            //row["Reason"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["reason"];

            //row["CarrierAccountId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["carrierAccountId"];
            //row["ShipmentType"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["shipmentType"];
            //row["CustomerId"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["customerId"];

            //row["LabelUrl"] = (string)jsonObject["responseObject"]?["additionalInfo"]?["labelUrl"];


            //// ShipFrom Info
            //row["ShipFromName"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromName"];
            //row["ShipFromAddress"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromAddress"];
            //row["ShipFromCity"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromCity"];
            //row["ShipFromState"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromState"];
            //row["ShipFromCountry"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromCountry"];
            //row["ShipFromPostalCode"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromPostalCode"];
            //row["ShipFromEmail"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromEmail"];
            //row["ShipFromMobile"] = (string)jsonObject["responseObject"]?["shipFromInfo"]?["shipFromMobile"];

            //// ShipTo Info
            //row["ShipToName"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToName"];
            //row["ShipToAddress"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToAddress"];
            //row["ShipToAddress2"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToAddress2"];
            //row["ShipToCity"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToCity"];
            //row["ShipToState"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToState"];
            //row["ShipToCountry"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToCountry"];
            //row["PostalCode"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["postalCode"];
            //row["ShipToEmail"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToEmail"];
            //row["ShipToMobile"] = (string)jsonObject["responseObject"]?["shipToInfo"]?["shipToMobile"];

            //// BillTo Info
            //row["BillToName"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToName"];
            //row["BillToAddress"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToAddress"];
            //row["BillToAddress2"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToAddress2"];
            //row["BillToCity"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToCity"];
            //row["BillToState"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToState"];
            //row["BillToCountry"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToCountry"];
            //row["BillToPostalCode"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToPostalCode"];
            //row["BillToEmail"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToEmail"];
            //row["BillToMobile"] = (string)jsonObject["responseObject"]?["billToInfo"]?["billToMobile"];

            //SqlContext.Pipe.Send("inserted BillToMobile: " + (string)jsonObject["responseObject"]?["billToInfo"]?["billToMobile"]);

            //trackingData.Rows.Add(row);


            SqlContext.Pipe.Send("inserted jsonBody: " + row);
            //}
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
