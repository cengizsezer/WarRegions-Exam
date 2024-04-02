public interface ITimerOwner
{
    void UpdateTime(float intervalTime);
    void ResumeTime(float passedRealtime);
}
