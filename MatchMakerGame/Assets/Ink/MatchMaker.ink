INCLUDE MatchMaker - New.ink

-> Start

=== Start
What do you want to do?
+ [Play Game]
    -> Reception_1
+ [Try out mechanics]
    -> LocationSelect
--> DONE

// VAR cardPicked = false
// VAR weirdFactor = 0
// -> LocationSelect
// -> Pick_Card_Loop

=== LocationSelect
Where to go now?#player
+ [Game]
    ->Reception_1
+ [reception]
    .#scene:reception
    Welcome to the reception. #narrator
    .#entrance:deer
    Oh hi there. 
    ++ [Test]
        -> rorschachTest
    ++ [Go away]
        #exit:other
        Weird. #player
        --->LocationSelect
+ [psychic]
    -> RikerPsychic
+ [cafe]
    .#scene:cafe
    Welcome to the cafe. #narrator
    // .#entrance:toad1
    // .#entrance:toad2
    .#entrance:toad1
    Ribbit! #exp:ribbit
    Hmmm.
    Ugh. #exp:frown
    Ugh. #exp:glitch
    ++ [swap frog]
    .#entrance:toad2
    Ribbit! #exp:ribbit
    Ugh. #exp:frown
    Ugh. #exp:glitch
    ++ [swap frog]
    .#entrance:toad3
    Ribbit! #exp:ribbit
    Ugh. #exp:frown
    Ugh. #exp:glitch
    -> LocationSelect
--Nice... #player
-> LocationSelect

=== RikerPsychic
.#scene:psychic
    Welcome to the psychic. #narrator
    .#entrance:owl
    Hoo!
    + [increase weirdness]
        ~weirdFactor++
        Weird factor is now {weirdFactor}. #player
        -> RikerPsychic
    + [leave]
        -> LocationSelect
    + [Tarot]
        ->Card_Loop
--> RikerPsychic

=== rorschachTest
Can I interest you in a test? 
    The doe pulls out a piece of paper. #narrator
    #test:1
    So what do you see here? #clearDialogue
    + A Plane #test:hoverPlane #player
    + A puppy #test:hoverPuppy #player
    -Yes, you do... #test:complete #exp:takenotes
    #test:2
    And now? #clearDialogue
    + Happiness? #player
    -Sure... #test:complete #exp:takenotes
-> LocationSelect


=== reception ===
You enter an unfamiliar place. #narrator
//  #entrance:deer
#entrance:player
Hi. #player
// #entrance:owl
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



== Card_Loop
#scene:psychic
.#clearDialogue
Are you ready? 
+ [Yes] #entrance:owl
    Hoo!
-The Great Glaucus {|picks up the cards and }fans cards in front of you{| again}. #narrator 
#cards:fan
Which of these speak to you?
+ From the left #cards:hoverLeft
    #cards:selectLeft
+ From the middle #cards:hoverMiddle
    #cards:selectMiddle
+ From the right #cards:hoverRight
    #cards:selectRight
- Let's see... #clearDialogue
{The card reads: The Imprisoned Man | The card reads: Life}
{cardPicked == true: -> After_Card | Oh, I don't know how that got in there. Let me do that again.}
#cards:discard
~cardPicked = true

-> Card_Loop

== After_Card
Ah, yes, life. The card tells me that you value life and the living. 
That's all. #cards:discard
-> LocationSelect
