using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Avatar Data", menuName = "CapsaBig2/Avatar Data", order = 0)]
public class AvatarData : ScriptableObject
{
    public enum State
    {
        Idle = 0,
        Happy = 1,
        Sad = -1
    }

    [System.Serializable]
    public class Emote
    {
        public string name = "None";
        public State state = State.Idle;
        public Sprite sprite = null;

        public Emote()
        {
            name = "None";
            state = State.Idle;
            sprite = null;
        }
    }

    [SerializeField] protected List<Emote> emotes = new List<Emote>();
    public Emote GetEmoteByState(State input)
    {
        return emotes.Where(x => x.state == input).FirstOrDefault();
    }
}
