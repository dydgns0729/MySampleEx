using UnityEngine;

namespace MySampleEx
{
    public enum MessageType
    {
        Damaged,
        Death,
        Respawn,
        //.... 
    }

    /// <summary>
    /// IMessageReceiver 상속받은 클래스에게만 메세지 타입 내용 전달
    /// </summary>
    public interface IMessageReceiver
    {
        void OnReceiveMessage(MessageType type, object sender, object msg);
    }
}