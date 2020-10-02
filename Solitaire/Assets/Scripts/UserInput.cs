using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitare solitare;

    private void Start()
    {
        solitare = FindObjectOfType<Solitare>();
        slot1 = this.gameObject;
    }

    private void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom")) 
                {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck() 
    {
        solitare.DealFromDeck();
    }

    void Card(GameObject selected) 
    {
        if (!selected.GetComponent<Selectable>().faceUp) //if the card clicked on is facedown
        {
            if (!Blocked(selected))
            {
                //if the card clicked on is not blocked
                // flip is over
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }
        }
        else if (selected.GetComponent<Selectable>().inDeckPile) // if the card clicked on is in the deck pile the trips
        {
            //if it is not blocked
            if (!Blocked(selected))
            {
                slot1 = selected;
            }
        }

        // if the card is face up
        // if there is no card currently selected
        // select the card

        if (slot1 == this.gameObject)
        {
            slot1 = selected;
        }

        // if there is already a card selected (and it is not the same card)

        else if (slot1 != selected)
        {
            // if the new card is eligable to stack on
            if (Stackable(selected))
            {
                Stack(selected);

            }
            else
            {
                // select the new card
                slot1 = selected;
            }
        }

        // else if there is already a card selected and it is the same card
        // if the time is short enough itis a double click
        //if the card isaligible to fly up the top then do it
    }
    void Top(GameObject selected) 
    {
        //top click actions
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 1)
            {
                Stack(selected);
            }
        }
    }   
    void Bottom(GameObject selected) 
    {
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }

    bool Stackable(GameObject selected) 
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();

        // compare them to see if they stack

        if (!s2.inDeckPile)
        {
            if (s2.top) // if in the top pile must stack suited Ace to King
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else // if in the bottom pile must stack alternate colours King to Ace
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        Debug.Log("Not stackable");
                        return false;
                    }
                    else
                    {
                        Debug.Log("Stakable");
                        return true;
                    }
                }
            }
        }
        return false;        
    }

    void Stack(GameObject selected) 
    {
        // if on top of king or empty bottom stack the cards in place
        // else stack the cards with negative y offset

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if (s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x,
            selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform; // this makes the children move with the parents

        if (s1.inDeckPile) //removes the cards from the top pile to prevent duplicate cards
        {
            solitare.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && s1.value == 1) // allows movement of cards between top spots
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top) //keeps track of the current value of the op deck as a card has been removed
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else // removes the card string from the appropriate bottom list
        {
            solitare.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; // you cannot add cards to the trips pile so this is always fine
        s1.row = s2.row;

        if (s2.top)
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }

        // after completing move reset slot1 to be essentialy null as being null will break the logic
        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected) 
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitare.tripsOnDisplay.Last())
            {
                return false;
            }
            else
            {
                print(s2.name + "is blocked by" + solitare.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitare.bottoms[s2.row].Last()) // check if it is the bottom card
            {
                return false;
            }
            else
            {
                return false;
            }
        }
    
    }

}
