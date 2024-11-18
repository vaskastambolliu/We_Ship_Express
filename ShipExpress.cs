using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace API_We_Ship_Express
{


    public class ResponseObject
    {
        [JsonProperty("responseStatus")]
        public bool responseStatus { get; set; }

        [JsonProperty("responseStatusCode")]
        public int responseStatusCode { get; set; }

        [JsonProperty("responseObject")]
        public OrderDetails responseObject { get; set; }
    }

    public class OrderDetails
    {
        [JsonProperty("orderNumber")]
        public OrderNumber orderNumber { get; set; }

        [JsonProperty("orderInfo")]
        public OrderInfo orderInfo { get; set; }

        [JsonProperty("additionalInfo")]
        public AdditionalInfo additionalInfo { get; set; }

        [JsonProperty("shipFromInfo")]
        public ShipFromInfo shipFromInfo { get; set; }

        [JsonProperty("shipToInfo")]
        public ShipToInfo shipToInfo { get; set; }

        [JsonProperty("billToInfo")]
        public BillToInfo billToInfo { get; set; }

        [JsonProperty("containerizedOrder")]
        public bool containerizedOrder { get; set; }

        [JsonProperty("multiItemOrders")]
        public int multiItemOrders { get; set; }

        [JsonProperty("shipstationOrder")]
        public int shipstationOrder { get; set; }

        [JsonProperty("orderTransactions")]
        public List<OrderTransaction> orderTransactions { get; set; }

        [JsonProperty("updateCarrier")]
        public bool updateCarrier { get; set; }

        [JsonProperty("rateCardEnable")]
        public bool rateCardEnable { get; set; }

        [JsonProperty("batchGenerated")]
        public bool batchGenerated { get; set; }

        [JsonProperty("orderType")]
        public string orderType { get; set; }

        [JsonProperty("icePack")]
        public int icePack { get; set; }

        [JsonProperty("orderDate")]
        public long orderDate { get; set; }

        [JsonProperty("ssPackingSlip")]
        public int ssPackingSlip { get; set; }

        [JsonProperty("ssShippingLabel")]
        public int ssShippingLabel { get; set; }

        [JsonProperty("productTypeDesc")]
        public string productTypeDesc { get; set; }

        [JsonProperty("carrierLabelUrl")]
        public string carrierLabelUrl { get; set; }

        [JsonProperty("connectorEmailStatus")]
        public int connectorEmailStatus { get; set; }

        [JsonProperty("skuGroup")]
        public string skuGroup { get; set; }

        [JsonProperty("fulfilment")]
        public string fulfilment { get; set; }
    }

    public class OrderNumber
    {
        [JsonProperty("id")]
        public string id { get; set; }
    }

    public class OrderInfo
    {
        [JsonProperty("createdOn")]
        public long createdOn { get; set; }

        [JsonProperty("updatedOn")]
        public long updatedOn { get; set; }

        [JsonProperty("parentOrder")]
        public string parentOrder { get; set; }

        [JsonProperty("orderType")]
        public int orderType { get; set; }

        [JsonProperty("orderTypeDesc")]
        public string orderTypeDesc { get; set; }

        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("orderStatusDesc")]
        public string orderStatusDesc { get; set; }

        [JsonProperty("accountId")]
        public string accountId { get; set; }

        [JsonProperty("clientName")]
        public string clientName { get; set; }

        [JsonProperty("companyName")]
        public string companyName { get; set; }

        [JsonProperty("shipperId")]
        public int shipperId { get; set; }

        [JsonProperty("warehouseLocation")]
        public string warehouseLocation { get; set; }

        [JsonProperty("updatedBy")]
        public int updatedBy { get; set; }

        [JsonProperty("totalQuantity")]
        public int totalQuantity { get; set; }

        [JsonProperty("totalWeight")]
        public float totalWeight { get; set; }

        [JsonProperty("paymentMode")]
        public int paymentMode { get; set; }

        [JsonProperty("paymentStatus")]
        public int paymentStatus { get; set; }

        [JsonProperty("serviceType")]
        public string serviceType { get; set; }

        [JsonProperty("shippingMode")]
        public string shippingMode { get; set; }

        [JsonProperty("lob")]
        public int lob { get; set; }

        [JsonProperty("lobStatus")]
        public string lobStatus { get; set; }

        [JsonProperty("storeId")]
        public string storeId { get; set; }

        [JsonProperty("storeName")]
        public string storeName { get; set; }
    }

    public class AdditionalInfo
    {
        [JsonProperty("channel")]
        public string channel { get; set; }

        [JsonProperty("referenceId")]
        public string referenceId { get; set; }

        [JsonProperty("deliveryTargetDate")]
        public long deliveryTargetDate { get; set; }

        [JsonProperty("deliveryTargetTime")]
        public string deliveryTargetTime { get; set; }

        [JsonProperty("trackingId")]
        public string trackingId { get; set; }

        [JsonProperty("internalTrackingId")]
        public string internalTrackingId { get; set; }

        [JsonProperty("boxId")]
        public string boxId { get; set; }

        [JsonProperty("boxCount")]
        public int boxCount { get; set; }

        [JsonProperty("orderPod")]
        public string orderPod { get; set; }

        [JsonProperty("carrier")]
        public string carrier { get; set; }

        [JsonProperty("labelPrinted")]
        public int labelPrinted { get; set; }

        [JsonProperty("notification")]
        public bool notification { get; set; }

        [JsonProperty("companyName")]
        public string companyName { get; set; }

        [JsonProperty("facilityName")]
        public string facilityName { get; set; }

        [JsonProperty("beginDate")]
        public string beginDate { get; set; }

        [JsonProperty("reason")]
        public string reason { get; set; }

        [JsonProperty("isAsnGenerated")]
        public bool isAsnGenerated { get; set; }

        [JsonProperty("orderState")]
        public int orderState { get; set; }

        [JsonProperty("otm")]
        public int otm { get; set; }

        [JsonProperty("carrierAccountId")]
        public string carrierAccountId { get; set; }

        [JsonProperty("shipmentType")]
        public string shipmentType { get; set; }

        [JsonProperty("customerId")]
        public string customerId { get; set; }

        [JsonProperty("isFulfilment")]
        public int isFulfilment { get; set; }

        [JsonProperty("label")]
        public int label { get; set; }

        [JsonProperty("labelUrl")]
        public string labelUrl { get; set; }

        [JsonProperty("isCCP")]
        public bool isCCP { get; set; }
    }

    public class ShipFromInfo
    {
        [JsonProperty("shipFromName")]
        public string shipFromName { get; set; }

        [JsonProperty("shipFromAddress")]
        public string shipFromAddress { get; set; }

        [JsonProperty("shipFromCity")]
        public string shipFromCity { get; set; }

        [JsonProperty("shipFromState")]
        public string shipFromState { get; set; }

        [JsonProperty("shipFromCountry")]
        public string shipFromCountry { get; set; }

        [JsonProperty("shipFromPostalCode")]
        public string shipFromPostalCode { get; set; }

        [JsonProperty("shipFromEmail")]
        public string shipFromEmail { get; set; }

        [JsonProperty("shipFromMobile")]
        public string shipFromMobile { get; set; }
    }

    public class ShipToInfo
    {
        [JsonProperty("shipToName")]
        public string shipToName { get; set; }

        [JsonProperty("shipToAddress")]
        public string shipToAddress { get; set; }

        [JsonProperty("shipToAddress2")]
        public string shipToAddress2 { get; set; }

        [JsonProperty("shipToCity")]
        public string shipToCity { get; set; }

        [JsonProperty("shipToState")]
        public string shipToState { get; set; }

        [JsonProperty("shipToCountry")]
        public string shipToCountry { get; set; }

        [JsonProperty("postalCode")]
        public string postalCode { get; set; }

        [JsonProperty("shipToEmail")]
        public string shipToEmail { get; set; }

        [JsonProperty("shipToMobile")]
        public string shipToMobile { get; set; }
    }

    public class BillToInfo
    {
        [JsonProperty("billToName")]
        public string billToName { get; set; }

        [JsonProperty("billToAddress")]
        public string billToAddress { get; set; }

        [JsonProperty("billToAddress2")]
        public string billToAddress2 { get; set; }

        [JsonProperty("billToCity")]
        public string billToCity { get; set; }

        [JsonProperty("billToState")]
        public string billToState { get; set; }

        [JsonProperty("billToCountry")]
        public string billToCountry { get; set; }

        [JsonProperty("billToPostal")]
        public string billToPostal { get; set; }

        [JsonProperty("billToEmail")]
        public string billToEmail { get; set; }

        [JsonProperty("billToMobile")]
        public string billToMobile { get; set; }
    }



    public class OrderTransaction
    {
        public string orderNumber { get; set; }

        [JsonProperty("status")]
        public int status { get; set; }

        [JsonProperty("createdOn")]
        public string createdOn { get; set; }

        [JsonProperty("updatedOn")]
        public string updatedOn { get; set; }

        [JsonProperty("reason")]
        public string reason { get; set; }

        [JsonProperty("orderStatusDesc")]
        public string orderStatusDesc { get; set; }

        [JsonProperty("statusSequence")]
        public int statusSequence { get; set; }

        [JsonProperty("shipToCity")]
        public string shipToCity { get; set; }

        [JsonProperty("shipToState")]
        public string shipToState { get; set; }
    }


}
