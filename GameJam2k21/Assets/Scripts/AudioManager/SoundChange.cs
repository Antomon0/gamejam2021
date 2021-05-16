
class SoundChange
{
    public float timeForChange;

    public float timeRemaining;

    public float newValue;

    public string paramToChange;

    public float initialValue;
    public Sound sound;
    public float diffValue;

    public bool isReverse = false;
    public SoundChange(float timeForChange, float newValue, string param, Sound s, float initialValue, bool isReverse = false)
    {
        this.timeForChange = timeForChange;
        this.timeRemaining = timeForChange;
        this.newValue = newValue;
        paramToChange = param;
        sound = s;
        this.initialValue = initialValue;
        diffValue = newValue - initialValue;
        this.isReverse = isReverse;
    }
}
