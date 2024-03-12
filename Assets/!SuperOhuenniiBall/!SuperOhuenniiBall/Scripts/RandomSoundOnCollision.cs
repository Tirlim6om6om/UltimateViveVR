using UnityEngine;

public class RandomSoundOnCollision : MonoBehaviour
{
    // Массив со звуками, которые будут воспроизводитьсяf
    public AudioClip[] sounds;

    // Компонент AudioSource, через который будут воспроизводиться звуки
    private AudioSource audioSource;

    // Индекс последнего проигранного звука
    private int lastPlayedSoundIndex = -1;

    void Start()
    {
        // Получаем компонент AudioSource при старте
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Вызывается, когда происходит столкновение
        PlayRandomSound();
    }

    void PlayRandomSound()
    {
        if (sounds.Length == 0) return; // Если массив звуков пуст, выходим из метода

        int newIndex = lastPlayedSoundIndex;

        // Выбираем случайный индекс звука, исключая последний проигранный
        while (newIndex == lastPlayedSoundIndex)
        {
            newIndex = Random.Range(0, sounds.Length);
        }

        // Воспроизводим звук с новым индексом
        audioSource.clip = sounds[newIndex];
        audioSource.Play();

        // Обновляем индекс последнего проигранного звука
        lastPlayedSoundIndex = newIndex;
    }
}