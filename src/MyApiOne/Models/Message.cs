using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyApiOne.Models
{


    public class Message
    {
        [Required]
        public string Title { get; init; } = default!;

        [Required]
        public string Body { get; init; } = default!;
        public MessageType Type { get; init; }

        [Required]
        public int SenderId { get; init; }
        public DateTime SendDate { get; set; }
        public int Provider { get; init; }

        public string Metadata { get; set; }
        public ICollection<Recipient> Recipients { get; set; } = new List<Recipient>();
    }

    public class Recipient
    {
        public long UserId { get; set; }
        public string Destination { get; set; }
    }


    public enum MessageType : byte
    {
        [Description("پیامک")]
        SMS = 1,

        [Description("ایمیل")]
        Email = 2,

        [Description("")]
        Rest = 3,

        [Description("نوتیف درون برنامه")]
        Signal = 4,

        [Description("")]
        MessageBrocker = 5,

        [Description("تلگرام")]
        Telegram = 6,

        [Description("واتسآپ")]
        Whatsapp = 7,

    }

}