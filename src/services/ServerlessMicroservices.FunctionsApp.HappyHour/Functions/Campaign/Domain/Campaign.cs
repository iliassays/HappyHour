namespace ServerlessMicroservices.FunctionsApp.HappyEating.Core.Domain.Campaign
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using ServerlessMicroservices.FunctionsApp.HappyEating.Core.IntegrationEvent;
    using System;
    using System.Collections.Generic;

    public class Campaign
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        public string ProductId { get; private set; } = "";

        public string Title { get; private set; }

        public string Description { get; private set; }

        public int NumberOfUnit { get; private set; }

        public int NumberOfAvailableUnit { get; private set; } = 0;

        public int NumberOfBookedItem { get; private set; } = 0;

        public string OrganizerId { get; private set; } = "";

        public string OrganizationName { get; private set; } = "";

        public DateTime CampaignBeginAt { get; private set; } = DateTime.UtcNow;

        public DateTime? CampaignEndAt { get; private set; } = null;

        public CampaignType CampaignType { get; private set; } = CampaignType.Normal;

        public CampaignStatus CampaignStatus { get; private set; } = CampaignStatus.New;

        protected Campaign()
        {
            
        }

        public Campaign(string id, string productId, string title, string description, int numberOfUnit, string organizerId, string organizationName) : this()
        {
            this.Id = id;
            this.ProductId = productId;
            this.Title = title;
            this.Description = description;
            this.NumberOfUnit = numberOfUnit;
            this.OrganizerId = organizerId;
            this.OrganizationName = organizationName;
        }

        public void UpdateCampaign(string title, string description, int numberOfUnit)
        {
            this.Title = title;
            this.Description = description;
            this.NumberOfUnit = numberOfUnit;
        }

        public void TryMapOrderCreatedEventToCampaign(OrderCreatedEvent orderCreatedEvent)
        {
            if (CampaignStatus == CampaignStatus.Running)
            {
                if (!hasAvailableUnitToOrder(orderCreatedEvent.NumberOfOrderedUnit))
                {
                    throw new Exception("Unfortunaltely we do not have available items to order. Try by reducing the unit.");
                }

                this.NumberOfAvailableUnit = this.NumberOfUnit - orderCreatedEvent.NumberOfOrderedUnit;

                if (this.NumberOfUnit == this.NumberOfAvailableUnit)
                {
                    this.SetCampaignStatusToFinished();
                }
            }
        }

        public CampaignStatus SetCampaignStatusToRunning()
        {
            if (CampaignStatus == CampaignStatus.New || CampaignStatus == CampaignStatus.Paused)
            {
                this.CampaignStatus = CampaignStatus.Running;
            }

            return this.CampaignStatus;
        }

        public CampaignStatus SetCampaignStatusToPaused()
        {
            if (CampaignStatus == CampaignStatus.Running)
            {
                this.CampaignStatus = CampaignStatus.Paused;
            }

            return this.CampaignStatus;
        }

        public CampaignStatus SetCampaignStatusToFinished()
        {
            if (CampaignStatus == CampaignStatus.Running)
            {
                if (NumberOfAvailableUnit < NumberOfUnit)
                {
                    this.CampaignStatus = CampaignStatus.FinishedWithUnsoldUnit;
                }
                else
                {
                    this.CampaignStatus = CampaignStatus.Finished;
                }
            }

            return this.CampaignStatus;
        }

        private bool hasAvailableUnitToOrder(int numberOfUnitOrdered)
        {
            return this.NumberOfAvailableUnit < numberOfUnitOrdered;
        }
    }

    public enum CampaignType
    {
        Normal,
        Demo
    }

    public enum CampaignStatus
    {
        New,
        Running,
        Paused,
        FinishedWithUnsoldUnit,
        Finished
    }
}