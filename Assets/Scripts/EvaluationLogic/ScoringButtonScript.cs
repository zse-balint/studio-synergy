using UnityEngine;

public class ScoringButtonScript : MonoBehaviour
{
    public Client Client { get; set; }

    public void TearDown()
    {
        Destroy(this.gameObject);
    }
}
