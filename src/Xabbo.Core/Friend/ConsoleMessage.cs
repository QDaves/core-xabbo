using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class ConsoleMessage : IParserComposer<ConsoleMessage>
{
    public Id ChatId { get; set; }
    public int MessageType { get; set; }
    public string Content { get; set; } = "";
    public int HabbiconId { get; set; }
    public int SecondsSinceSent { get; set; }
    public string? Time { get; set; }
    public string MessageId { get; set; } = "";
    public int ConfirmationId { get; set; }
    public Id SenderId { get; set; }
    public string? SenderName { get; set; }
    public string SenderFigure { get; set; } = "";

    public ConsoleMessage() { }

    private ConsoleMessage(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            MessageId = p.ReadString();
            SenderId = p.ReadId();
            Time = p.ReadString();
            Content = p.ReadString().Replace('\r', '\n');
        }
        else
        {
            ChatId = p.ReadId();
            MessageType = p.ReadInt();
            switch (MessageType)
            {
                case 0:
                    Content = p.ReadString();
                    break;
                case 1:
                    HabbiconId = p.ReadInt();
                    break;
            }
            SecondsSinceSent = p.ReadInt();
            MessageId = p.ReadString();
            ConfirmationId = p.ReadInt();
            SenderId = p.ReadId();
            SenderName = p.ReadString();
            SenderFigure = p.ReadString();
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteString(MessageId);
            p.WriteId(SenderId);
            p.WriteString(Time ?? "");
            p.WriteString(Content.Replace('\n', '\r'));
        }
        else
        {
            p.WriteId(ChatId);
            p.WriteInt(MessageType);
            switch (MessageType)
            {
                case 0:
                    p.WriteString(Content);
                    break;
                case 1:
                    p.WriteInt(HabbiconId);
                    break;
            }
            p.WriteInt(SecondsSinceSent);
            p.WriteString(MessageId);
            p.WriteInt(ConfirmationId);
            p.WriteId(SenderId);
            p.WriteString(SenderName ?? "");
            p.WriteString(SenderFigure);
        }
    }

    static ConsoleMessage IParser<ConsoleMessage>.Parse(in PacketReader p) => new(in p);
}