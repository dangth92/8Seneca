namespace EightSeneca.Wpf.Contracts
{
    public interface IMediaService
    {
        void PlayVideo(int index, string path);
        void StopVideo(int index);
    }
}
