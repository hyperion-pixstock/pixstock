namespace Pixstock.Service.App.Model
{
    public class CategoryParam
    {
        public static readonly string LLA_ORDER_NAME_ASC = "NAME_ASC";
        
        public static readonly string LLA_ORDER_NAME_DESC = "NAME_DESC";

        public bool IsAlbum { get; set; }

        public string lla_order { get; set; }
    }
}