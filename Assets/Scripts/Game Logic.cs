using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLogic : MonoBehaviour
{
    // Start is called before the first frame update
    private int playerCt = 0;
    private int dealerCt = 0;
    public List<Sprite> deck;
    public List<Sprite> cardBacks;
    private List<Sprite> playerHand = new List<Sprite>();
    private List<Sprite> dealerHand = new List<Sprite>();
    private List<GameObject> cardsOutPlayer = new List<GameObject>();
    private List<GameObject> cardsOutDealer = new List<GameObject>();
    bool dealerStand = false;
    bool playerLost = false;
    bool playerWon = false;
    bool tie = false;
    bool gameOver = false;
    public TMP_Text winText;
    public TMP_Text playerPoints;
    public TMP_Text dealerPoints;
    public GameObject defaultCard;
    private float xStartP = -5;
    private float xStartD = 5;
    private int layerCount = 0;
    private Sprite hiddenCard;
    private GameObject hiddenObject;
    private bool firstCard = true;
    Sprite color;
    public GameObject coin;
    public List<GameObject> deckObj;
    //public List<Sprite> testHand;
    void Start()
    {
      if(PlayerPrefs.GetInt("blue") == 1) { color = cardBacks[1]; }
      else if(PlayerPrefs.GetInt("black") == 1) { color = cardBacks[2]; }
      else { color = cardBacks[0]; } //red default if all are 0
      defaultCard.GetComponent<SpriteRenderer>().sprite = color;
      foreach(GameObject card in deckObj) {
        card.GetComponent<SpriteRenderer>().sprite = color;
      }
      PlayerDraw();
      DealerDraw();
      PlayerDraw();
      DealerDraw(); //initial hands pulled
    }

    void FixedUpdate()
    {
      if(gameOver) {
        hiddenObject.GetComponent<SpriteRenderer>().sprite = hiddenCard;
        dealerPoints.text = "Dealer Points: " + dealerCt;
      }
      if(playerLost) { winText.text = "Dealer Wins!"; }
      else if(playerWon) {
        winText.text = "Player Wins!";
        GameObject newCoin = Instantiate(coin, new Vector3(UnityEngine.Random.Range(-10, 10), 5, 0), Quaternion.identity);
        cardsOutPlayer.Add(newCoin);
      }
      else if(tie) { winText.text = "Push"; }
    }
    public void HitOrReset() {
      if(gameOver) {
        Debug.Log("Reset");
        GameReset();
      }
      else {
        PlayerDraw();
        //DealerBehavior();
      }
    }
    void GameReset() {
      gameOver = false;
      winText.text = "";
      //resuffle hands back into the mix!
      foreach(Sprite card in playerHand) {
        deck.Add(card);
      }
      foreach(Sprite card in dealerHand) {
        deck.Add(card);
      }
      foreach(GameObject card in cardsOutPlayer) {
        Destroy(card);
      }
      foreach(GameObject card in cardsOutDealer) {
        Destroy(card);
      }
      playerHand.Clear();
      dealerHand.Clear();
      dealerStand = false;
      playerLost = false;
      playerWon = false;
      firstCard = true;
      tie = false;
      playerCt = 0;
      dealerCt = 0;
      layerCount = 0;
      xStartP = -5; xStartD = 5;
      Debug.Log("All Cleared");
      PlayerDraw();
      DealerDraw();
      PlayerDraw();
      DealerDraw(); //initial hands pulled
    }

    void PlayerDraw() {
      int index = UnityEngine.Random.Range(0, deck.Count);
      Sprite card = deck[index];
      GameObject newCard = Instantiate(defaultCard, new Vector3(xStartP, -2.5f, 0), Quaternion.identity);
      xStartP+=0.75f;
      newCard.GetComponent<SpriteRenderer>().sprite = card;
      newCard.GetComponent<SpriteRenderer>().sortingOrder = layerCount;
      cardsOutPlayer.Add(newCard);
      layerCount++;
      playerHand.Add(card); //
      deck.Remove(card);
      playerCt = SumHand(playerHand);
      //Debug.Log("Player: " + playerCt);
      playerPoints.text = "Player Points: " + playerCt;
      if(playerCt > 21) { playerLost = true; gameOver = true; }
      else if(playerCt == 21) { playerWon = true; gameOver = true; }
    }

    public void PlayerStand() { //other win conditions here
      hiddenObject.GetComponent<SpriteRenderer>().sprite = hiddenCard; //dealer reveals hidden card
      if(!dealerStand) {
        while(!dealerStand) {
          DealerBehavior();
        }
      }
      if(playerCt > dealerCt || dealerCt > 21) { playerWon = true; gameOver = true; } //since player bust = lose game by default, no need to check here
      else if(playerCt == dealerCt) { tie = true; gameOver = true; }
      else { playerLost = true; gameOver = true; }
    }

    public void DealerDraw() {
      int index = UnityEngine.Random.Range(0, deck.Count);
      Sprite card = deck[index];
      GameObject newCard = Instantiate(defaultCard, new Vector3(xStartD, 2.5f, 0), Quaternion.identity);
      xStartD-=0.75f;
      newCard.GetComponent<SpriteRenderer>().sortingOrder = layerCount;
      if(firstCard) {
        firstCard = false;
        hiddenCard = card;
        hiddenObject = newCard;
      }
      else {
        newCard.GetComponent<SpriteRenderer>().sprite = card;
      }
      cardsOutDealer.Add(newCard);
      layerCount++;
      dealerHand.Add(card);
      deck.Remove(card);
      dealerCt = SumHand(dealerHand);
      //Debug.Log("Dealer: " + dealerCt);
      dealerPoints.text = "Dealer Points: " + (dealerCt - Value(hiddenCard));
      if(dealerCt > 21) { playerWon = true; gameOver = true; }
      else if(dealerCt == 21) { playerLost = true; gameOver = true; }
    }

    public void DealerBehavior() {
      if(dealerCt < 17 && !gameOver) { DealerDraw(); }
      else { dealerStand = true; }
    }

    int SumHand(List<Sprite> hand) { //figure out aces
      int count = 0;
      int aceCount = 0;
      foreach(Sprite card in hand) {
        String value = card.name.Split(" ")[1];
        int number;
        bool success = int.TryParse(value, out number);
        if(success) {
          if(number == 1) { aceCount++; }
          else if(number < 10) { count += number; }
          else { count += 10; }
        }
      }
      if(aceCount == 0) { return count; }
      else if(aceCount == 1) {
        if(count+11 <= 21) { count += 11; }
        else{ count += 1; }
        return count;
      }
      else {
        int original = count;
        for(int i = 0; i < aceCount; i++) {
          if(i == 0) { count+=11; } //toggle first ace to 11
          else { count+=1; } //rest are 1
        }
        if(count > 21) { return original + aceCount; } //if bust then toggle all to 1 to get the smallest value
        else { return count; } //otherwise return count
      }
    }

    int Value(Sprite card) {
      String value = card.name.Split(" ")[1];
      int number;
      bool success = int.TryParse(value, out number);
      if(success) { return number; }
      else { return 0; }
    }
}