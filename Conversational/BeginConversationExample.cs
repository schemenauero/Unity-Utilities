using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginConversationExample : MonoBehaviour {
    public Conversational conversational;

    Button button;

    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(BeginConversation);
    }

    void BeginConversation() {
        Debug.Log("Begin Conversation Example Button Clicked");
        Conversation c = new Conversation();
        c.Add("start", new List<ConversationEvent>() {
            new Say("Hail, Creator. Tell me, how are you?"),
            new Response("I am good and don't need any further help.", "end"),
            new Response("I am confused. Please explain how this system works.", "how_it_works"),
            new Response("I just want to play with delegates (check the code to see this in action).", "end", delegate {
                Debug.Log("Delegate called. Side effects here.");
            }),
            new Response("I really want to play with delegates (check the code to see this in action).", "end", OnSelect),
        });
        c.Add("how_it_works", new List<ConversationEvent>() {
            new Say("No problem. It's a simple system, defined in code. You start by adding the Conversational Prefab to your Canvas. " +
                "Next, call a series of functions to set up your conversation. You can see how this example works in the BeginConversationExample.cs"),
            new Response("Ok, thanks; I get it.", "end"),
            new Response("What if I want to customize how the UI looks?", "how_it_looks"),
        });
        c.Add("how_it_looks", new List<ConversationEvent>() {
            new Say("You can change how the Background, SpeakerText, and PlayerText children of the Conversational prefab look using the standard Unity UI tools."),
            new Response("Thanks!", "end"),
        });
        conversational.BeginConversation(c);
    }

    private void OnSelect(Response response) {
        Debug.Log("This is a delegate function defined elsewhere");
        Debug.Log("You selected the response with text: " + response.text);
    }
}
