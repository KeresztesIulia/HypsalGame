using UnityEngine;

public class script_GameEntry : MonoBehaviour
{
    [Tooltip("For now, GameObject should have all the PersistentData interfaces, not gonna search for child components")]
    [SerializeField] GameObject[] _instanciatiables;
    private void Awake()
    {
        foreach (var instanciatiable in _instanciatiables)
        {
            var instance = Instantiate(instanciatiable);
            foreach (var component in instance.GetComponents<MonoBehaviour>())
            {
                if (component is interface_PersistentData) // can't really order these, aside from by whole object... which sucks honestly
                {
                    (component as interface_PersistentData).Initialize();
                }

            }
        }


    }
}
