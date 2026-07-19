VAR cardPicked = false
-> Pick_Card_Loop
=== receptionist ===
You enter an unfamiliar place. #narrator
//  #entrance:deer
#entrance:player
Hi. #player
#entrance:owl
Well, hello there!
Welcome to Match Maker. #exp:smile
Thanks. #player
Nice to meet you. #exp:frown
What's your name? #exp:takenotes
* [button text] my name is button. #player
    sjedfkjsenfg#player
    dslfkmv#player
* [I don't wan't to talk]
    I don't wan't to talk #player
* [I'm ok actually]
    I'm ok actually #player
-That's it. 
Still wondering what's going on, huh? #narrator
Oh, really? #player
Wow.
* [Challenge]
    // -> knot
* [Accept]
-Whatever.
-> DONE

=== expressionLoop
Here I am. #player
#entrance:player
Said the player. And made a choice... #narrator
Ok. #player
+ [Doe]
    #entrance:deer
    Hello!
+ [Owl]
    #entrance:owl
    Hello!
-Hello!
-> expr
-> DONE

=== expr 
+ [Happy]
    Yay! #player #exp:smile
+ [Sad]
    Aww. #player #exp:frown
+ [raised Eyebrow (doe)]
    Huh? #player #exp:raisedEyebrow
+ [Notes]
    Interesting... #player #exp:takenotes
+ [glitch]
    Bzzzz #player #exp:glitch
+ [change character]
    -> expressionLoop
--> expr
-> DONE



== Pick_Card_Loop
.#entrance:owl
Are you ready? #clearDialogue
The Great Glaucus {|picks up the cards and }fans cards in front of you{| again}. #narrator #cards:fan
Which of these speak to you?
+ From the left #cards:hoverLeft
    #cards:selectLeft
+ From the middle #cards:hoverMiddle
    #cards:selectMiddle
+ From the right #cards:hoverRight
    #cards:selectRight
- Let's see... #clearDialogue
{The card reads: The Imprisoned Man | The card reads: Life}
{cardPicked == true: -> After_Card_Picks | Oh, I don't know how that got in there. Let me do that again.} #cards:discard
~cardPicked = true

->Pick_Card_Loop

== After_Card_Picks
Ah, yes, life. The card tells me that you value life and the living. 
That's all. #cards:discard
-> DONE
