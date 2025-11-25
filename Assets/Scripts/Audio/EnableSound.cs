using System.Security.Cryptography;
using UnityEngine;

public class EnableSound : MonoBehaviour
{
    [SerializeField] private AudioClip enableSound;
    private void OnEnable()
    {
        AudioManager.Instance.PlaySound(enableSound, 0.8f);
    }
}
