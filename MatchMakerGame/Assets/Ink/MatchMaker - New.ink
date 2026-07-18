VAR insight = 0
VAR receptionist = 0
VAR psychicOwl = 0
VAR nonbeliever = false
VAR cardPicked = false
VAR questioningTheProcess = 0
VAR weirdFactor = 0

-> scene1
=== scene1 ===
.#entrance:player
A Match Made in Meadows. This place has a bit of a reputation for unorthodox methods, but you have no other options. #narrator
#entrance:deer
Can I help you?
Oh, hello. I didn't see you there. #player
Are you here for a screening?
* Screening? #player
The intake questionnaire.
Oh. Yes. #player
* That's right. #player
- Excellent. Welcome to A Match Made in Meadows. #exp:smile
Let's begin. I'm going to say a word, then you tell me the first word you think of.
* Okay. #player
* [Shouldn't you ask my name?] Shouldn't you ask my name or something? #player
Stop questioning the process. First word: <> #exp:raisedeyebrow
~ questioningTheProcess = questioningTheProcess + 1
- Love.
* Hate. #player
* Heart. #player
* Trust. #player
- Hmm. #exp:takenotes
Blue.
* Ocean. #player
* Sad. #player
* Red. #player
- Hmmmm. #exp:takenotes
Escape.
* Freedom. #player
* Trapped. #player
* Futile. #player
- Hmmmmmmm. #exp:takenotes
->Alone_loop
= Alone_loop
Alone.
* Lonely. #player
* Together. #player
* How is this relevant? #player
One word only, please. #exp:raisedeyebrow
-> Alone_loop
- ->Madness_loop
= Madness_loop
Madness.
* Inevitable. #player
* What? #player
I said: <>
->Happiness_loop
- Hmmm. Interesting. #exp:takenotes
-> After_Happiness_Loop


= Happiness_loop
Happiness.
** [No, you didn't.] No you didn't. You said madness.  #player
Only one-word answers, please. #exp:frown
~ insight = insight + 1
->Happiness_loop
** Joy. #player
** Peace. #player
-- I see.
->After_Happiness_Loop

= After_Happiness_Loop


Alright. Next, let's get to know you a bit. What are your hobbies?
* Swimming. #player
* [Building watercraft.] Uh... building watercraft, I suppose. #player
* Cooking on an open fire.  #player
- Great. #exp:takenotes

And your living situation? Are you planning to stay where you are forever? Are you planning to do travelling soon?
* [I'll never leave.] I think I'm comfortable here for now. #player
* I'll leave here as soon as possible. #player
* [Not sure.] I don't know yet. #player
- Good. Thank you. 



That's it for the intake questionnaire. Next we'll need you to see the psychic.
* I'm ready. #player
-> Psychic
* [Nothing about what I'm looking for?] You're not going to ask anything about what kind of person I'm looking for? #player
-> What_Im_Looking_For
* I don't believe in psychics. #player

- ->Psychic

= What_Im_Looking_For
You need to trust the process, or you'll be alone in this place forever. #exp:raisedeyebrow
~ questioningTheProcess = questioningTheProcess + 1
* [Sorry.] Sorry, it's just a bit strange. #player
* [Whatever.] Fine, but I hope someone asks me what I want before this is over. #player
- -> Psychic

= Dont_Believe
That doesn't matter. The Great Glaucus doesn't need you to believe. Go on.
* Fine. #player
~ nonbeliever = true
- -> Psychic








=== Psychic ===
The psychic's room is dark, and a bit cramped. #narrator
#entrance:owl
Hoo-hoo! #exp:glitch
* Hello?
- Oh, yes, hello! Excuse the mess. 
* Uhh  #player
- Not that I'm surprised that you're here or anything. The Great Glaucus knows all.
Ah, yes. I can see it. You are here to be assessed. Your innards exposed for destiny to see. #exp:takenotes
* {nonbeliever} [I know this is a sham.] No need to pretend with me. I know this is a sham.  #player
~ psychicOwl = psychicOwl - 1
A non-believer! Disappointing. Nonetheless, the Great Glaucus will dig into your soul and pull from it what is needed.
-> Pick_Card_Loop
* I'd rather not have my innards exposed.  #player
* That's right.  #player
- Be not afraid. The Great Glaucus will safely carry you through the waves of time.
-> Pick_Card_Loop
== Pick_Card_Loop
The Great Glaucus {|picks up the cards and }puts {|another} three cards in front of you. #narrator
Which of these speak to you?
+ [Left]
+ [Middle]
+ [Right]
- {The card reads: The Imprisoned Man | The card reads: Life}
{cardPicked == true: -> After_Card_Picks | Oh, I don't know how that got in there. Let me do that again.} 
~cardPicked = true


->Pick_Card_Loop

== After_Card_Picks
Ah, yes, life. The card tells me that you value life and the living. 
* [Does this help me?] Is this going to help you match me? #player
Ah, I see your doubt, but you must perservere on this path. Learning about you will help us learn about the one you are meant to be with. So says the Great Glaucus!
* [That's true!] I do care about life and the living! #player
Of course you do! So it is written!
~ psychicOwl = psychicOwl + 1
* [Imprisoned Man?] What does the Imprisoned Man mean? #player
Do not worry yourself about that. It means nothing. It was not supposed to be in there. You can trust the Great Glaucus!
- Now, let me look into my crystal ball, and see your future!


{psychicOwl == -1:
I can see a figure, glimmering in the light! Someone you rely on. Who gives you hope. Gives you courage!
Ah, a bright future indeed! #exp:smile
-> Normal_Future_Read
}
{psychicOwl == 0:
A cloud obscures my view, but I can see a silhouette! Your partner, waiting for you! I can see you, hesitating on the edge of the cloud.
Ah, your hesitation is holding you back. Your love is waiting for you to step over the threshold! #exp:smile
-> Normal_Future_Read
-else:
Hoo-hoo! #exp:glitch
A landscape vast and empty! Oh, the sorrow! Lone soul, begging deliverance from the forest! #exp:frown
Hoo-hoo! #exp:glitch
Ah yes, a bright future indeed! #exp:smile
-> Glitch_Future_Read
}
== Normal_Future_Read
* Can you tell me more about this figure?
* When do I get to meet them?
Ah, patience! We shall match you with your partner before long!
- -> Last_Psychic_Questions
== Glitch_Future_Read
* [What about the sorrow?] What do you mean, a bright future? What was all that about sorrow?
Sorrow? I don't believe I know what you mean! A bright future with a wonderful partner awaits! Never doubt that. So says the Great Glaucus!
* A bright future sounds great!
~ insight = insight - 1

- ->Last_Psychic_Questions
== Last_Psychic_Questions

Ah, wait! #exp:takenotes
Your future partner is trying to connect to me via the paths of destiny. But I need your help! Concentrate on your match, and send that energy to me via the crystal ball! What do you see? #exp:takenotes
* A brave soul
* A kind soul
* [A lonely island] A lo... a loving soul.
~ insight = insight + 1

- Yes, I see it! All is done. Return from whence you came, and you will be matched with the one destiny has chosen for you. So says the Great Glaucus.
->Interim

== Interim
//{insight}
You return to reception. #narrator
#entrance:doe
The Great Glaucus has passed on his findings.
// can do an exchange about a negative relationship score with the owl

//{psychicOwl}
* [You have my match?] He said you'll have my match. Do you? #player

* {psychicOwl <= 0} [I'm excited!]  I'm excited to meet the match he saw in his visions! #player

* {psychicOwl > 0} [He said strange things.] He said some strange things. Something about sorrow and an empty landscape. #player
What was that about? #player
The ways of the Great Glaucus are mysterious.
There is nothing to worry about. #exp:glitch
~ insight = insight + 1
{questioningTheProcess > 0: 
I thought I told you to stop questioning the process. #exp:frown
}
{questioningTheProcess == 0: 
Stop questioning the process. #exp:raisedeyebrow
}
 The future you've always wanted is possible if you stop fighting it.
- You don't have to wait much longer. We have found your perfect match.
Wait here while we prepare your date.
#exit:other
The doe leaves you alone in the waiting room. #narrator
->Waiting_In_The_Waiting_Room
== Waiting_In_The_Waiting_Room
+ [Wait patiently]
->Wait_Patiently
+ [Leave the agency]
->Leave_Agency(->Waiting_In_The_Waiting_Room)
+ [Look around]
->Look_Around_Waiting_Room

= Wait_Patiently
You sit down and wait to be called. #narrator

= Look_Around_Waiting_Room
There are a couple of plants here, some of them coming out of the walls. #narrator
* [Leave them be.]
->Wait_Patiently
* [Check plants.]
- 
== Leave_Agency(->return_to)
{insight > 10:
You hear someone behind you, but urgency pushes you forward and you yank open the door. #narrator
->END
}
-else:
You try the door, but it doesn't budge. #narrator
#entrance:doe
Where are you going? #exp:frown
* [I'm leaving.] I'm leaving. What about it?
* [Just looking around.] Nowhere. I'm just looking around.
- You are in a very fragile state, so we can't let you leave at this moment. Please stay in the waiting room until we call you.
#exit:doe
->return_to
}







->END



















