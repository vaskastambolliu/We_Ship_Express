using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace API_We_Ship_Express
{
   

    public class OrderNumber
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class OrderInfo
    {
        [JsonProperty("createdOn")]
        public long CreatedOn { get; set; }

        [JsonProperty("updatedOn")]
        public long UpdatedOn { get; set; }

        [JsonProperty("parentOrder")]
        public string ParentOrder { get; set; }

        [JsonProperty("orderType")]
        public int OrderType { get; set; }

        [JsonProperty("orderTypeDesc")]
        public string OrderTypeDesc { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("orderStatusDesc")]
        public string OrderStatusDesc { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("shipperId")]
        public int ShipperId { get; set; }

        [JsonProperty("warehouseLocation")]
        public string WarehouseLocation { get; set; }

        [JsonProperty("updatedBy")]
        public int UpdatedBy { get; set; }

        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }

        [JsonProperty("totalWeight")]
        public double TotalWeight { get; set; }

        [JsonProperty("paymentMode")]
        public int PaymentMode { get; set; }

        [JsonProperty("paymentStatus")]
        public int PaymentStatus { get; set; }

        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }

        [JsonProperty("shippingMode")]
        public string ShippingMode { get; set; }

        [JsonProperty("lob")]
        public int Lob { get; set; }

        [JsonProperty("lobStatus")]
        public string LobStatus { get; set; }

        [JsonProperty("storeId")]
        public string StoreId { get; set; }

        [JsonProperty("storeName")]
        public string StoreName { get; set; }
    }

    public class AdditionalInfo
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("deliveryTargetDate")]
        public int DeliveryTargetDate { get; set; }

        [JsonProperty("deliveryTargetTime")]
        public string DeliveryTargetTime { get; set; }

        [JsonProperty("trackingId")]
        public string TrackingId { get; set; }

        [JsonProperty("internalTrackingId")]
        public string InternalTrackingId { get; set; }

        [JsonProperty("boxId")]
        public string BoxId { get; set; }

        [JsonProperty("boxCount")]
        public int BoxCount { get; set; }

        [JsonProperty("orderPod")]
        public string OrderPod { get; set; }

        [JsonProperty("carrier")]
        public string Carrier { get; set; }

        [JsonProperty("labelPrinted")]
        public int LabelPrinted { get; set; }

        [JsonProperty("notification")]
        public bool Notification { get; set; }

        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        [JsonProperty("facilityName")]
        public string FacilityName { get; set; }

        [JsonProperty("beginDate")]
        public string BeginDate { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("isAsnGenerated")]
        public bool IsAsnGenerated { get; set; }

        [JsonProperty("orderState")]
        public int OrderState { get; set; }

        [JsonProperty("otm")]
        public int Otm { get; set; }

        [JsonProperty("carrierAccountId")]
        public string CarrierAccountId { get; set; }

        [JsonProperty("shipmentType")]
        public string ShipmentType { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("isFulfilment")]
        public int IsFulfilment { get; set; }

        [JsonProperty("label")]
        public int Label { get; set; }

        [JsonProperty("labelUrl")]
        public string LabelUrl { get; set; }

        [JsonProperty("isCCP")]
        public bool IsCcp { get; set; }
    }

    public class ShipFromInfo
    {
        [JsonProperty("shipFromName")]
        public string ShipFromName { get; set; }

        [JsonProperty("shipFromAddress")]
        public string ShipFromAddress { get; set; }

        [JsonProperty("shipFromCity")]
        public string ShipFromCity { get; set; }

        [JsonProperty("shipFromState")]
        public string ShipFromState { get; set; }

        [JsonProperty("shipFromCountry")]
        public string ShipFromCountry { get; set; }

        [JsonProperty("shipFromPostalCode")]
        public string ShipFromPostalCode { get; set; }

        [JsonProperty("shipFromEmail")]
        public string ShipFromEmail { get; set; }

        [JsonProperty("shipFromMobile")]
        public string ShipFromMobile { get; set; }
    }

    public class ShipToInfo
    {
        [JsonProperty("shipToName")]
        public string ShipToName { get; set; }

        [JsonProperty("shipToAddress")]
        public string ShipToAddress { get; set; }

        [JsonProperty("shipToAddress2")]
        public string ShipToAddress2 { get; set; }

        [JsonProperty("shipToCity")]
        public string ShipToCity { get; set; }

        [JsonProperty("shipToState")]
        public string ShipToState { get; set; }

        [JsonProperty("shipToCountry")]
        public string ShipToCountry { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("shipToEmail")]
        public string ShipToEmail { get; set; }

        [JsonProperty("shipToMobile")]
        public string ShipToMobile { get; set; }
    }

    public class BillToInfo
    {
        [JsonProperty("billToName")]
        public string BillToName { get; set; }

        [JsonProperty("billToAddress")]
        public string BillToAddress { get; set; }

        [JsonProperty("billToAddress2")]
        public string BillToAddress2 { get; set; }

        [JsonProperty("billToCity")]
        public string BillToCity { get; set; }

        [JsonProperty("billToState")]
        public string BillToState { get; set; }

        [JsonProperty("billToCountry")]
        public string BillToCountry { get; set; }

        [JsonProperty("billToPostal")]
        public string BillToPostal { get; set; }

        [JsonProperty("billToEmail")]
        public string BillToEmail { get; set; }

        [JsonProperty("billToMobile")]
        public string BillToMobile { get; set; }
    }

    public class OrderTransaction
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("createdOn")]
        public string CreatedOn { get; set; }

        [JsonProperty("updatedOn")]
        public string UpdatedOn { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("orderStatusDesc")]
        public string OrderStatusDesc { get; set; }

        [JsonProperty("statusSequence")]
        public int StatusSequence { get; set; }

        [JsonProperty("shipToCity")]
        public string ShipToCity { get; set; }

        [JsonProperty("shipToState")]
        public string ShipToState { get; set; }
    }

    public class ResponseObject
    {
        [JsonProperty("orderNumber")]
        public OrderNumber OrderNumber { get; set; }

        [JsonProperty("orderInfo")]
        public OrderInfo OrderInfo { get; set; }

        [JsonProperty("additionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }

        [JsonProperty("shipFromInfo")]
        public ShipFromInfo ShipFromInfo { get; set; }

        [JsonProperty("shipToInfo")]
        public ShipToInfo ShipToInfo { get; set; }

        [JsonProperty("billToInfo")]
        public BillToInfo BillToInfo { get; set; }

        [JsonProperty("containerizedOrder")]
        public bool ContainerizedOrder { get; set; }

        [JsonProperty("multiItemOrders")]
        public int MultiItemOrders { get; set; }

        [JsonProperty("shipstationOrder")]
        public int ShipstationOrder { get; set; }

        [JsonProperty("orderTransactions")]
        public List<OrderTransaction> OrderTransactions { get; set; }

        [JsonProperty("updateCarrier")]
        public bool UpdateCarrier { get; set; }

        [JsonProperty("rateCardEnable")]
        public bool RateCardEnable { get; set; }

        [JsonProperty("batchGenerated")]
        public bool BatchGenerated { get; set; }

        [JsonProperty("orderType")]
        public string OrderType { get; set; }

        [JsonProperty("icePack")]
        public int IcePack { get; set; }

        [JsonProperty("orderDate")]
        public long OrderDate { get; set; }

        [JsonProperty("connectorEmailStatus")]
        public int ConnectorEmailStatus { get; set; }

        [JsonProperty("skuGroup")]
        public string SkuGroup { get; set; }
    }

    public class Root
    {
        [JsonProperty("responseStatus")]
        public bool ResponseStatus { get; set; }

        [JsonProperty("responseStatusCode")]
        public int ResponseStatusCode { get; set; }

        [JsonProperty("responseObject")]
        public ResponseObject ResponseObject { get; set; }
    }

}
