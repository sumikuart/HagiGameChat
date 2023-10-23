namespace SessionService.Interface
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
