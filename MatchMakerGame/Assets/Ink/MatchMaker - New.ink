VAR insight = 0
VAR receptionist = 0
VAR psychicOwl = 0
VAR toad = 0
VAR nonbeliever = false
VAR cardPicked = false
VAR questioningTheProcess = 0
VAR weirdFactor = 0
VAR playerHobby = ""
VAR nrDates = 0
VAR bottlesCollected = 0
VAR hasReceptionBottle = false
VAR hasPsychicBottle = false
VAR hasCafeBottle = false
VAR askedAboutFlies = false
VAR triedToLeave = false
VAR toldAboutIsolation = false
//Scenes:
//reception
//psychic
//cafe

-> Reception_1

== function Inc_Insight
~ insight = insight + 1
~Calculate_WeirdFactor()

== function Calculate_WeirdFactor
{
    - insight >= 14:
        ~ weirdFactor = 6
    - insight >= 12:
        ~weirdFactor = 5
    - insight >= 10:
        ~weirdFactor = 4
    - insight >= 8:
        ~weirdFactor = 3
    - insight >= 6:
        ~weirdFactor = 3
    - insight >= 4:
        ~weirdFactor = 2
    - insight >= 2:
        ~weirdFactor = 1
    - else:
        ~weirdFactor = 0
        
    
}



=== Reception_1 ===
#scene:reception
.#entrance:player
A Match Made in Meadows. This place has a bit of a reputation for unorthodox methods, but you have no other options. #narrator
#entrance:deer
Can I help you?
* Oh, hello. I didn't see you there. #player
- Are you here for a screening?
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
Madness. #destroy
* Inevitable. #player
* Excuse me? #player
    Happiness. #replace
    I said: <>
    ->Happiness_loop
- Hmmm. Interesting. #exp:takenotes
-> After_Happiness_Loop


= Happiness_loop
Happiness.
* [No, you didn't.] No you didn't. You said madness.  #player
    Only one-word answers, please. #exp:frown
    {Inc_Insight()}
    ->Happiness_loop
* Joy. #player
* Peace. #player
- I see.
->After_Happiness_Loop

= After_Happiness_Loop


Alright. Next, let's get to know you a bit. What are your hobbies?
* Swimming. #player
    ~ playerHobby = "swimming"
* [Building watercraft.] Uh... building watercraft, I suppose. #player
    ~ playerHobby = "building watercraft"
* Cooking on an open fire.  #player
    ~ playerHobby = "cooking"
- Great. #exp:takenotes

And your living situation? Are you planning to stay where you are forever? Are you planning on travelling soon?
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



//-----------------------------------------------------------
//-----------------------------------------------------------






=== Psychic ===
#scene:psychic
The psychic's room is dark, and a bit cramped. #narrator

Hoo-hoo! #exp:glitch #entrance:owl
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
The Great Glaucus {|picks up the cards and }fans cards in front of you{| again}. #narrator #clearDialogue #cards:fan
Which of these speak to you?
+ From the left #cards:hoverLeft
    #cards:selectLeft
+ From the middle #cards:hoverMiddle
    #cards:selectLeft
+ From the right #cards:hoverRight
    #cards:selectLeft
- {The card reads: Imprisonment | The card reads: Life} #narrator
#clearDialogue
{cardPicked == true: -> After_Card_Picks | Oh, I don't know how that got in there. Let me do that again.} 
.#cards:discard
~cardPicked = true


->Pick_Card_Loop

== After_Card_Picks
.#cards.discard
Ah, yes, life. The card tells me that you value life and the living. 
* [Does this help me?] Is this going to help you match me? #player
    Ah, I see your doubt, but you must perservere on this path. 
    Learning about you will help us learn about the one you are meant to be with. 
    So says the Great Glaucus!
* [That's true!] I do care about life and the living! #player
    Of course you do! So it is written!
    ~ psychicOwl = psychicOwl + 1
* [Imprisonment] What does the Imprisonment card mean? #player
    Do not worry yourself about that. It means nothing. It was not supposed to be in there. 
    You can trust the Great Glaucus!
- Now, let me look into my crystal ball, and see your future!


{psychicOwl == -1:
    I can see a figure, glimmering in the light! Someone you rely on. Who gives you hope. Gives you courage!
    Ah, a bright future indeed! #exp:smile
    -> Normal_Future_Read
}
{psychicOwl == 0:
    A cloud obscures my view, but I can see a silhouette! 
    Your partner, waiting for you! I can see you, hesitating on the edge of the cloud.
    Ah, your hesitation is holding you back. 
    Your love is waiting for you to step over the threshold! #exp:smile
    -> Normal_Future_Read
-else:
    Hoo-hoo! #exp:glitch
    A landscape vast and empty! 
    Oh, the sorrow! 
    Lone soul, begging deliverance from the forest! #exp:frown
    Hoo-hoo! #exp:glitch
    Ah yes, a bright future indeed! #exp:smile
-> Glitch_Future_Read
}
== Normal_Future_Read
* Can you tell me more about this figure? #player
* When do I get to meet them? #player
Ah, patience! We shall match you with your partner before long!
- -> Last_Psychic_Questions
== Glitch_Future_Read
* [What about the sorrow?] What do you mean, a bright future? What was all that about sorrow? #player
Sorrow? I don't believe I know what you mean! A bright future with a wonderful partner awaits! Never doubt that. So says the Great Glaucus!
~Inc_Insight()
* A bright future sounds great! #player

- ->Last_Psychic_Questions
== Last_Psychic_Questions

Ah, wait! #exp:takenotes
Your future partner is trying to connect to me via the paths of destiny. 
But I need your help! 
Concentrate on your match, and send that energy to me via the crystal ball! What do you see? #exp:takenotes
* A brave soul
* A kind soul
* [A lonely island] A lo... a loving soul.
~Inc_Insight()

- Yes, I see it! All is done. 
Return from whence you came, and you will be matched with the one destiny has chosen for you. 
So says the Great Glaucus.
->Interim

== Interim
//{insight}
.#scene:reception
You return to reception. #narrator
.#entrance:doe
The Great Glaucus has passed on his findings.
// can do an exchange about a negative relationship score with the owl

//{psychicOwl}
* [You have my match?] He said you'll have my match. Do you? #player

* {psychicOwl <= 0} [I'm excited!]  I'm excited to meet the match he saw in his visions! #player

* {psychicOwl > 0} [He said strange things.] He said some strange things. Something about sorrow and an empty landscape. #player
What was that about? #player
The ways of the Great Glaucus are mysterious.
There is nothing to worry about. #exp:glitch
{Inc_Insight()}
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
-> Waiting_In_The_Waiting_Room ->
- Your date is ready. Follow me. #entrance:doe
.#exit:other
-> Date_1

== Waiting_In_The_Waiting_Room
+ [Wait patiently]
    ->Wait_Patiently
+ {insight <= 6} [Check door]
    ->Leave_Agency(->Waiting_In_The_Waiting_Room.Wait_Patiently)
+ {insight > 6 && bottlesCollected < 3 && triedToLeave} [Check escape]
    ->Leave_Agency(->Waiting_In_The_Waiting_Room.Wait_Patiently)
+ {bottlesCollected == 3} [Escape]
    ->Leave_Agency(->Waiting_In_The_Waiting_Room.Wait_Patiently)    
+ {not hasReceptionBottle} {insight >=4} [Look around]
    ->Look_Around_Waiting_Room

-
->DONE

= Wait_Patiently
You sit down and wait to be called. #narrator
+ [...]
- 
+ [...]

->->
//->Date_1

= Look_Around_Waiting_Room
There are a couple of plants here, some of them coming out of the walls. #narrator

{hasReceptionBottle == false:
    //~insight = 9
    {insight >= 8:
        Between two chairs, you find a small glass bottle, filled with white sand. #narrator
        * [Take bottle of sand.]
            The sand smells like the ocean.
            You put the bottle in your pocket. #narrator
            ~hasReceptionBottle = true
            ~bottlesCollected = bottlesCollected + 1
            ->Waiting_In_The_Waiting_Room
        
            
    }
}


+ [Check plants.]
    You feel one of the vines coming from the walls. It is slightly warm to the touch, growing out of a crack in the ceiling. #narrator
    There is something scratched on the wall.
    "Get out. Three bottles break three locks."
    //could add something to take here? Flower on the vine?
    ->Waiting_In_The_Waiting_Room
    
+ [Return to your seat.]
    ->Waiting_In_The_Waiting_Room



//-----------------------------------------------------------
//-----------------------------------------------------------


==Date_1
~ temp flies = false
~ nrDates = nrDates + 1
#scene:cafe
#entrance:player
The doe ushers you into {a|the} small cafe. #narrator
#entrance:toad1
The toad watches you.
+ Hello. #player
    Hello.
+ [Not saying anything?] Are you not going to say anything? #player
    #exp:frown
    Hello.
+ (just_met) {nrDates > 1} You again? #player
    We've never met. #exp:glitch
    ** You weren't here before?[] A couple of minutes ago? #player
        No.
        *** Is this [a joke?] some kind of joke? 
        Nope.
    ** Oh, nevermind.[] I suppose you just look familiar. #player
        Okay.
    -- Okay.
+ (me_again) {just_met} It's me again.[] New hat, huh? #player
    Just my normal hat. #exp:glitch
+ Nice to meet you. #player
    Yup.
- 



+ So you're my match[.], huh? #player
    Yup.
+ How's it going? #player
    Good.
* What's your name? #player
    {Theodore.|Teddy.|Tom.|Todd.}
- 
//{insight}

+ Did you go through a screening? #player
    Yup.
    ** [What hobby did you give?] What did you say your hobby was? #player
        Flies.
        ~ flies = true
        ~askedAboutFlies = true
        *** Cool. #player
        *** [Flies?] Your hobby is flies? #player
            Yup.
            **** [Elaborate?] Do you want to elaborate? #player
                No, I'm good.
            **** I see. Okay. #player
            ----
        ---
    --
+ I like your hat. #player
    Okay.
    ** Where did you get it? #player
        In a store.
    ** It suits you. #player
        Thanks.
    ** I had a hat like that[.] once. #player
        Nice.
    --
+ {insight >= 3} [This place is strange.] This place sure is strange, right? #player
    Okay.
    ** You don't think so?[] A lot of the questions don't make sense. #player
        How does that help them match us? #player
        I don't know.
    ** Something is off. #player
        Okay.
        *** I'm serious.[] It's like things aren't the way they seem. #player
            Okay.
    --
-
+ [You don't look how I imagined.] You look different from how I imagined my soulmate. #player
    Oh.
    ** [Not bad, just different.] I don't mean you look bad. You're just different than the person I imagined in my head. #player
        Okay.
    ** [Didn't imagine a nice hat.] I didn't imagine you'd have such a nice hat. #player
        Thanks.
    --
+ [Why did they match us?] Why do you think they matched us? #player
    There's no one else in this place. #exp:glitch #destroy
    ** What do you mean[?] there's no one else? What are you talking about?
        I didn't say that.
        I don't know. #replace
        *** Yeah, you did. #player
            Ribbit. #exp:glitch
            Okay.
            **** [Leave cafe] I'm going to leave now. #player
                Okay.
                -> End_Of_Date_1
            **** [Let's start over.] Okay, fine. Let's just start over. #player
            ----
        *** I guess I misheard.[] Okay, let's just start over. #player
        --- Okay.
+ [Leave cafe] I'm going to leave now. #player
    Okay.
    -> End_Of_Date_1
-


+ Do you like {playerHobby}? #player
    Not really.
+ What's your favourite food?[] {flies==true:Is it flies?|}  #player
    Flies.
    ~askedAboutFlies = true
    ** {flies == true} I knew it. #player
        Nice.
    ** {flies == false} Just... just flies? #player
        Yup.
    ** Me too! #player
        Nice.
    --
-
* [I should get back.] Well, I guess I should get back. #player
-
-> End_Of_Date_1



->DONE

== End_Of_Date_1
#scene:reception
You return to the reception with the date fresh in mind. #narrator
#entrance:doe
How did it go?
* It was terrible. #player
    Sorry to hear that. What happened?
    ** It was boring. #player
    ** It was maddening. #player
    ** {nrDates > 1} It was the same person[.] as last time. #player
        No, it was a new match. #exp:glitch
        Now that we've identified your type, you may find that your matches will have some similarities.
        {Inc_Insight()}
    -- 
    
* It was great! #player
    Ah. #exp:takenotes
    It seems your match didn't feel the same way.
    Sorry.
    ** What did I do? #player
        They said you were apparently a bit too boring.
        *** <i>I</i> was too boring? #player
            Try not to take it too hard.
            Given your long isolation, it's the best you can come up with. #destroy
            ~toldAboutIsolation = true
            **** What do you mean[?], my isolation? #player
                I'm not sure I follow. I said that there's someone out there for you.
                There's someone out there for you, and we'll find them. #replace
                ***** That's not what you said.[] #player
                    {Inc_Insight()}
                    {questioningTheProcess > 0: 
                        You need to stop. Questioning the process can only bring you pain.
                        {questioningTheProcess > 1:
                            Everything will unravel.
                        }
                    }
                ***** [I hope so.] I hope that's true. That there's someone out there.
            **** I suppose so. #player
            ----
    ** Oh well. #player
    --
-
Enough about that.
Perhaps we should refine your match search a bit. 
->Refinement


== Refinement
{
- nrDates == 1:
    Let's try the adcanced image questionnaire.
    ->Advanced_Questionnaire    

- nrDates == 2:
    You should visit the Great Glaucus again.
    ->Advanced_Psychic_Reading
- else:
    NO CONTENT HERE YET
    ->END
}

->DONE

= Advanced_Questionnaire
Wait here while I prepare some things.
#exit:other

-> Waiting_In_The_Waiting_Room ->

#entrance:doe
Alright, let's begin.
What do you see in this image? It may be unclear, so just say the first thing that comes to mind.
* (plane) Plane crashing into an island. #player
* (puppy) Puppy. #player
- 
Oh. Wow. Okay, sure. #exp:takenotes
{plane:
    I wonder if you remember it. #exp:glitch #destroy
} 
* {plane} Remember what?[] #player
    Have you drifted off? I asked if you were ready for the next one.
    Are you ready for the next one? #replace
    {Inc_Insight()}
    ** I guess so. #player
    --
* Is that wrong? #player
-

What about this image?
* A person screaming. #player
* A person in love. #player
-
I see. #exp:takenotes
Interesting.

Okay, that's it for the advanced questionnaire. 

Wait here while I prepare your next date.
-> Waiting_In_The_Waiting_Room ->
->Date_2

->DONE

= Advanced_Psychic_Reading
NO CONTENT HERE YET
->END



-> END

//-----------------------------------------------------------
//-----------------------------------------------------------


==Date_2
~ temp flies = false
~ nrDates = nrDates + 1
#scene:cafe
#entrance:player
The doe ushers you back into the small cafe. #narrator
#entrance:toad2
Greetings and salutations. 
I am here, at your service.
+ (just_met) {nrDates > 1} You again? #player
    We've never met, I assure you. #exp:glitch
    ** You weren't here before?[] A couple of minutes ago? #player
        I was not. 
        Surely I would remember a person such as you.
        So I assure you, this is our first meeting.
        *** Is this [a joke?] some kind of joke? #player
            I would not jest.
            Rest assured, I shall be nothing but honest with you. #exp:glitch
    ** Oh, nevermind.[] I suppose you just look familiar. #player
        I have that kind of face.
    -- If you say so. #player
//+ (me_again) {just_met} It's me again.[] New hat, huh? #player
//    Just my normal hat. #exp:glitch
+ Nice to meet you. #player
    The pleasure is mine.
- 

* {askedAboutFlies} What's your favourite fly?
    Ah, I could not possibly choose!
    The humble blowfly, so sweet and delightful.
    A crafty scavenger, born to find the tastiest morsels.
    Ah, but the oft-forgotten fruitfly, taken with citrus and berry.
    A beautiful dancer, flitting from fruit to fruit.
    But forget not the mighty horsefly!
    It takes what it wants, even from giants!
    How could I possibly choose between these glorious insects?
    Each hold a special place in my heart!
* [Where are you from?] Are you from around here?
    I am from the world! 
    To the edges of the sea, wherever flies can be found!
    Those beautiful creatures, everywhere.
    I can't stop thinking about them!
    ** So you've travelled a lot[?] then?
        Oh, I've seen so many flies!
        Blowflies, horseflies, fruitflies.
        Fleshflies, cranefiles, sandflies.
        Ah.
    ** But do you live here?
        There are flies everywhere!
        Living in harmony with the world.
        Flying about in a beautiful dance.
        Ah.
    -- 
    ** You sure do like flies[.], huh? 
        Of course!
        How could I not!
        They are the most incredible thing!
    --
//* question about the matchmakers etc.
-
* {insight > 8} [This place is coming apart.] By the way, have you noticed that this place is coming apart? 
    Not at all!
    It is sturdy and entirely normal! #exp:glitch
    ** Do you know something?
        {Inc_Insight()}
        N-not at all! There is nothing to know, you see!
        Did you know that flies taste with their feet?
        
    ** You're lying!
        {Inc_Insight()}
        I am most certainly not! #exp:frown
        If I'm lying, then you're lying! #exp:glitch #destroy
        *** What am I lying about?
            I didn't say you're lying.
            I would never lie to you! #replace
            **** Tell me what's going on!
                N-nothing is going on! Leave me be!
                ->Toad_Runs_Away(->Waiting_In_Cafe)
        *** Tell me the truth.
            I-I have to go!
            ->Toad_Runs_Away(->Waiting_In_Cafe)
    ** I guess you're right.
    --
 

* [I should get back] Well, I guess I should get back. #player
-
-> End_Of_Date_2

=Toad_Runs_Away(->return_to)
The toad runs away, leaving you alone in the cafe. #narrator


->DONE

==Waiting_In_Cafe
* [Return to reception]
    -> End_Of_Date_2
* {not hasCafeBottle} [Look around]
    The cafe is small, but nice. On a shelf, you find a small glass bottle with a piece of paper rolled up inside.
    ** [Take bottle]
    - You take the bottle. Unrolling the paper inside, you see it reads "Help me, I'm stranded". 
    ~Inc_Insight()
    You return the paper and put the bottle in your pocket.
    ~hasCafeBottle = true
    ~bottlesCollected = bottlesCollected + 1
    ->Waiting_In_Cafe

->DONE


== End_Of_Date_2
#scene:reception
You return to the reception with the date fresh in mind. #narrator
#entrance:doe

//END OF CONTENT FOR NOW
//->END
=How_It_Went_Loop
{How did it go?|And? How was it?}
* {nrDates > 1} It was the same person[.] as last time. #player
        No, it was a new match. #exp:glitch
        Now that we've identified your type, you may find that your matches will have some similarities.
        {Inc_Insight()}
        ->How_It_Went_Loop
* It was terrible. #player
    Sorry to hear that. What happened?
    ** [Only talked about flies.] They spent the whole time going on about flies. #player
    ** They talked too much.[] Didn't ask me a single question. #player
    -- 
* It was great! #player
    Ah. #exp:takenotes
    It seems your match didn't feel the same way.
    Sorry.
    ** Why? #player
        They said you didn't seem interested enough in them and their interests.
        *** They didn't even ask me what I think! #player
        --- Try not to take it too hard.
        Given your long isolation, a bit of rambling is normal. #destroy
            *** {toldAboutIsolation == false} What do you mean[?], my isolation? #player
                I'm not sure I follow. I said that there's someone out there for you.
                There's someone out there for you, and we'll find them. #replace
                **** That's not what you said.[] #player
                    {Inc_Insight()}
                    {questioningTheProcess > 0: 
                        You need to stop. Questioning the process can only bring you pain.
                        {questioningTheProcess > 1:
                            Everything will unravel.
                        }
                    }
                    
                **** [I hope so.] I hope that's true. That there's someone out there. #player
            *** {toldAboutIsolation == true} [Isolation again?] You said the same thing last time. #player
                That I've had a long isolation. What does that mean? #player
                I'm not sure I follow. I said that there's someone out there for you.
                There's someone out there for you, and we'll find them. #replace
                **** I don't believe you.[] #player
                    {Inc_Insight()}
                    Please. Stop.
                    Everything will unravel. #exp:glitch
                    
                **** [I hope so.] I hope that's true. That there's someone out there. #player
            *** I suppose so. #player
            ---
    ** Oh well. #player
    --
-
We're getting off track. 
Perhaps we should refine your match search a bit. 
->Refinement

//-----------------------------------------------------------
//-----------------------------------------------------------


==Date_3
~ temp flies = false
~ nrDates = nrDates + 1
#scene:cafe
#entrance:player
The doe ushers you back into the small cafe. #narrator
#entrance:toad2
Greetings and salutations. 
I am here, at your service.
+ (just_met) {nrDates > 1} You again? #player
    We've never met, I assure you. #exp:glitch
    ** You weren't here before?[] A couple of minutes ago? #player
        I was not. 
        Surely I would remember a person such as you.
        So I assure you, this is our first meeting.
        *** Is this [a joke?] some kind of joke? #player
            I would not jest.
            Rest assured, I shall be nothing but honest with you. #exp:glitch
    ** Oh, nevermind.[] I suppose you just look familiar. #player
        I have that kind of face.
    -- If you say so. #player
//+ (me_again) {just_met} It's me again.[] New hat, huh? #player
//    Just my normal hat. #exp:glitch
+ Nice to meet you. #player
    The pleasure is mine.
- 

* {askedAboutFlies} What's your favourite fly?
    Ah, I could not possibly choose!
    The humble blowfly, so sweet and delightful.
    A crafty scavenger, born to find the tastiest morsels.
    Ah, but the oft-forgotten fruitfly, taken with citrus and berry.
    A beautiful dancer, flitting from fruit to fruit.
    But forget not the mighty horsefly!
    It takes what it wants, even from giants!
    How could I possibly choose between these glorious insects?
    Each hold a special place in my heart!
* [Where are you from?] Are you from around here?
    I am from the world! 
    To the edges of the sea, wherever flies can be found!
    Those beautiful creatures, everywhere.
    I can't stop thinking about them!
    ** So you've travelled a lot[?] then?
        Oh, I've seen so many flies!
        Blowflies, horseflies, fruitflies.
        Fleshflies, cranefiles, sandflies.
        Ah.
    ** But do you live here?
        There are flies everywhere!
        Living in harmony with the world.
        Flying about in a beautiful dance.
        Ah.
    -- 
    ** You sure do like flies[.], huh? 
        Of course!
        How could I not!
        They are the most incredible thing!
    --
//* question about the matchmakers etc.
-
* {insight > 8} [This place is coming apart.] By the way, have you noticed that this place is coming apart? 
    Not at all!
    It is sturdy and entirely normal! #exp:glitch
    ** Do you know something?
        {Inc_Insight()}
        N-not at all! There is nothing to know, you see!
        Did you know that flies taste with their feet?
        
    ** You're lying!
        {Inc_Insight()}
        I am most certainly not! #exp:frown
        If I'm lying, then you're lying! #exp:glitch #destroy
        *** What am I lying about?
            I didn't say you're lying.
            I would never lie to you! #replace
            **** Tell me what's going on!
                N-nothing is going on! Leave me be!
                ->Toad_Runs_Away(->Waiting_In_Cafe)
        *** Tell me the truth.
            I-I have to go!
            ->Toad_Runs_Away(->Waiting_In_Cafe)
    ** I guess you're right.
    --
 

* [I should get back] Well, I guess I should get back. #player
-
-> End_Of_Date_2

=Toad_Runs_Away(->return_to)
The toad runs away, leaving you alone in the cafe. #narrator


->DONE


== Leave_Agency(->return_to)
~triedToLeave = true
{bottlesCollected == 3:
You hear someone behind you, but the three locks are open, so you yank open the door and rush outside. #narrator
->END

-else:
You try the door, but it doesn't budge. #narrator
{
    - bottlesCollected == 0:
        There are three locks, keeping it closed. #narrator        
    - bottlesCollected == 1:
        There are two locks left, keeping it closed. #narrator        
    - bottlesCollected == 2:
        There is one lock left, keeping it closed. #narrator        
}

}
#entrance:doe
Where are you going? #exp:frown
* [I'm leaving.] I'm leaving. What about it? #player
* [Just looking around.] Nowhere. I'm just looking around. #player
- You are in a very fragile state, so we can't let you leave at this moment. Please stay in the waiting room until we call you.
#exit:doe
->->
->return_to
}




->END



















