namespace HappyEating.Campaign.Core.Command
{
    public class SaveCampaignCommand
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int NumberOfUnit { get; set; }

        public int NumberOfAvailableUnit { get; set; } = 0;

        public int NumberOfBookedItem { get; set; } = 0;

        public string OrganizerId { get; set; }

        public string OrganizationName { get; set; }

        public string PictureName { get; set; }

        public string PictureUri { get; set; }

        public string DetailsUri { get; set; }

    }
}
