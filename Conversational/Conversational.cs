using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Conversational : MonoBehaviour {
    public TMP_Text speakerText;
    public TMP_Text playerText;
    public Image[] backgrounds;

    public float backgroundFadeSpeed = 0.02f;
    public float typewriterWaitTime = 0.0001f;

    Conversation currentConversation;
    string currentKey = "start";
    bool isTyping = false;

    List<float> finalBackgroundAlphas;

    public void BeginConversation(Conversation c) {
        StopAllCoroutines();
        currentKey = "start";
        currentConversation = c;
        StartCoroutine(PlayConversation());
    }

    IEnumerator PlayConversation() {
        FadeInBackgrounds();
        while (currentKey != "end") {
            string staticKey = currentKey;
            isTyping = true;
            yield return StartCoroutine(PlayConversationAtKey(currentKey));
            isTyping = false;
            while (staticKey == currentKey) { yield return new WaitForSeconds(0.03f); }
        }
        speakerText.SetText("");
        playerText.SetText("");
        FadeOutBackgrounds();
    }

    IEnumerator PlayConversationAtKey(string key) {
        speakerText.SetText("");
        playerText.SetText("");

        // speaker text
        string finalSpeakerText = currentConversation.GetSayEventString(key);
        IEnumerator playSpeakerText = PlayText(finalSpeakerText, speakerText);
        yield return StartCoroutine(playSpeakerText);

        // player text
        List<Response> responses = currentConversation.GetResponseEvents(key);
        string finalPlayerText = "";
        for (int n = 0; n < responses.Count; n++) {
            Response response = responses[n];
            finalPlayerText += (n+1) + ". " + response.text + "\n";
        }
        IEnumerator playPlayerText = PlayText(finalPlayerText, playerText);
        yield return StartCoroutine(playPlayerText);
    }

    IEnumerator PlayText(string finalText, TMP_Text textGameObject) {
        string currentSpeakerText = "";
        foreach (char c in finalText) {
            currentSpeakerText += c;
            textGameObject.SetText(currentSpeakerText);
            yield return new WaitForSeconds(typewriterWaitTime);
        }
    }

    void FadeInBackgrounds() {
        for (int n = 0; n < backgrounds.Length; n++) {
            StartCoroutine(FadeInBackground(backgrounds[n], finalBackgroundAlphas[n]));
        }
    }

    IEnumerator FadeInBackground(Image background, float finalAlpha) {
        while (background.color.a < finalAlpha) {
            background.color = new Color(background.color.r, background.color.g, background.color.b, background.color.a + backgroundFadeSpeed);
            yield return new WaitForSeconds(0.01f);
        }    
    }

    void FadeOutBackgrounds() {
        for (int n = 0; n < backgrounds.Length; n++) {
            StartCoroutine(FadeOutBackground(backgrounds[n]));
        }
    }

    IEnumerator FadeOutBackground(Image background) {
        while (background.color.a > 0) {
            background.color = new Color(background.color.r, background.color.g, background.color.b, background.color.a - backgroundFadeSpeed);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void Start() {
        speakerText.SetText("");
        playerText.SetText("");
        finalBackgroundAlphas = new List<float>();
        foreach (Image background in backgrounds) {
            finalBackgroundAlphas.Add(background.color.a);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
        }
    }

    public void Update() {
        if (!isTyping) {
            int optionSelected = -1;
            if (Input.GetButtonDown("FirstSelection"))  optionSelected = 0;
            if (Input.GetButtonDown("SecondSelection")) optionSelected = 1;
            if (Input.GetButtonDown("ThirdSelection"))  optionSelected = 2;
            if (Input.GetButtonDown("FourthSelection")) optionSelected = 3;
            if (Input.GetButtonDown("FifthSelection"))  optionSelected = 4;
            if (Input.GetButtonDown("SixthSelection"))  optionSelected = 5;
            if (optionSelected >= 0) {
                Response selectedResponse = currentConversation.GetResponseEvents(currentKey)[optionSelected];
                selectedResponse.onSelect?.Invoke(selectedResponse);
                currentKey = selectedResponse.nextKey;
            }
        }
    }
}

public class Conversation {
    Dictionary<string, List<ConversationEvent>> data;

    public Conversation() {
        data = new Dictionary<string, List<ConversationEvent>>();
    }

    public void Add(string key, List<ConversationEvent> events) {
        data.Add(key, events);
    }

    public string GetSayEventString(string key) {
        if (key == "end") return "";
        if (!data.ContainsKey(key)) throw new System.InvalidOperationException("Conversation doesn not contain key: " + key);
        string eventString = "";
        foreach (ConversationEvent e in data[key]) {
            if (e.GetType() == typeof(Say)) {
                Say s = (Say)e;
                eventString += s.text;
            }
        }
        return eventString;
    }

    public List<Response> GetResponseEvents(string key) {
        if (key == "end") return new List<Response>();
        if (!data.ContainsKey(key)) throw new System.InvalidOperationException("Conversation doesn not contain key: " + key);
        List<Response> responses = new List<Response>();
        foreach(ConversationEvent e in data[key]) {
            if (e.GetType() == typeof(Response)) {
                Response r = (Response)e;
                responses.Add(r);
            }
        }
        return responses;
    }
}

public class ConversationEvent {}

public class Say : ConversationEvent {
    public string text;
    public Say(string _text) { text = _text; }
}

public class Response : ConversationEvent {
    public string text;
    public string nextKey;

    public delegate void OnSelect(Response response);
    public OnSelect onSelect;

    public Response(string _text, string _nextKey) {
        text = _text;
        nextKey = _nextKey;
    }

    public Response(string _text, string _nextKey, OnSelect _onSelect) {
        text = _text;
        nextKey = _nextKey;
        onSelect = _onSelect;
    }
}