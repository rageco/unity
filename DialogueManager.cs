using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class FormatList
{
  public char triggerchar;
  public bool charactive;
  public string begintag;
  public string endtag;
}

public class DialogueManager : MonoBehaviour
{
  public Text maintext;
  public new Text name;
  public Dialogue dialogue;
  public CharactersList characters;
  public Image character;
  public Image autobutton;
  public AudioSource source;
  public Button continuebutton;
  public FormatList[] chars;
  public Sprite autoactivesp;
  public Sprite autoinactivesp;
  public float autotime;
  public float defaulttypingspeed;
  public float typingspeed;
  public float speedfactor = 1f;
  public string desiredline;
  public bool continueactive;
  public bool skip;
  public bool auto;
  public int index;
  public float defaultbuttonpausetime = 0.3f;
  public char formatchar;


  public void Start()
  {
    StartCoroutine(TypeLine(false));
  }

  public void Update()
  {
    if(continueactive)
    {
      continuebutton.interactable = true;
    }
    else
    {
      continuebutton.interactable = false;
    }

    if(auto && autobutton.sprite != autoactivesp)
    {
      autobutton.sprite = autoactivesp;
    }
    else if(!auto && autobutton.sprite != autoinactivesp)
    {
      autobutton.sprite = autoactivesp;
    }

    if(Input.GetKeyDown("space"))
    {
      Continue();
    }
  }


  IEnumerator TypeLine(bool continued)
  {
    float speed = 0f;
    float buttonpausetime = 0.1f;
    bool format = false;
    char[] letters = dialogue.dialogues[index].line.ToCharArray();
    string storestring = "";
    string basetext = "";
    FormatList[] reversechars = new FormatList[chars.Length];
    skip = false;

    continueactive = false;
    continuebutton.interactable = false;

    //speedfactor manager
    if(dialogue.speedoverride && !dialogue.dialogues[index].advanced.canceloverride)
    {
      speedfactor = dialogue.speedfactor;
    }
    else if(dialogue.dialogues[index].advanced.customfactor)
    {
      speedfactor = dialogue.dialogues[index].advanced.speedfactor;
    }
    else
    {
      speedfactor = 1f;
    }

    //button pause time manager
    if(!dialogue.dialogues[index].advanced.buttonpausetimeoverride)
    {
      buttonpausetime = dialogue.dialogues[index].advanced.buttonpausetime;
    }
    else
    {
      buttonpausetime = defaultbuttonpausetime;
    }

    //character speed customization
    if(!dialogue.dialogues[index].advanced.customspeed)
    {
      speed = characters.characters[dialogue.dialogues[index].charactercode].typingspeed;
    }
    else
    {
      speed = dialogue.dialogues[index].advanced.typingspeed;
    }
    if(speed == 0f)
    {
      speed = typingspeed;
    }

    //set sound
    source.clip = characters.characters[dialogue.dialogues[index].charactercode].typesound;

    //set name
    if(name.text != characters.characters[dialogue.dialogues[index].charactercode].name)
    {
      name.text = characters.characters[dialogue.dialogues[index].charactercode].name;
    }

    //set sprite
    if(characters.characters[dialogue.dialogues[index].charactercode].protagonist)
    {
      if(dialogue.dialogues[index].listener.enabled)
      {
        SetSprite(characters.characters[dialogue.dialogues[index].listener.charactercode].sprites[dialogue.dialogues[index].listener.moodcode]);
      }
      else
      {
        if(CheckIndex(false))
        {
          SetSprite(characters.characters[dialogue.dialogues[index - 1].charactercode].sprites[dialogue.dialogues[index - 1].moodcode]);
        }
        else if(CheckIndex(true))
        {
          SetSprite(characters.characters[dialogue.dialogues[index + 1].charactercode].sprites[dialogue.dialogues[index + 1].moodcode]);
        }
      }
    }
    else if(character.sprite != characters.characters[dialogue.dialogues[index].charactercode].sprites[dialogue.dialogues[index].moodcode])
    {
      SetSprite(characters.characters[dialogue.dialogues[index].charactercode].sprites[dialogue.dialogues[index].moodcode]);
    }

    //type
    for(int i=0; i < letters.Length; i++)
    {
      char letter = letters[i];
      foreach(FormatList letter1 in chars)
      {
        int letterindex = Array.FindIndex(chars, chars => chars == letter1);
        reversechars[reversechars.Length - letterindex - 1] = letter1;
      }

      if(letter == formatchar)
      {
        if(!format)
        {
          format = true;
          basetext = maintext.text;
          storestring = "";
          CheckChar(letters[i + 1]);
        }
        else
        {
          format = false;
          basetext = "";
          storestring = "";
          foreach(FormatList letter1 in chars)
          {
            letter1.charactive = false;
          }
        }
      }
      else if(CheckSkip(letter))
      {
        continue;
      }
      else if(format)
      {
        maintext.text = basetext;
        storestring += letter;
        foreach(FormatList letter1 in chars)
        {
          if(letter1.charactive)
          {
            maintext.text += letter1.begintag;
          }
        }
        maintext.text += storestring;
        foreach(FormatList letter1 in reversechars)
        {
          if(letter1.charactive)
          {
            maintext.text += letter1.endtag;
          }
        }
        if(!skip)
        {
          yield return new WaitForSeconds(speed * speedfactor);
        }
        source.Play();
      }
      else
      {
        storestring = "";
        basetext = "";
        maintext.text += letter;
        if(!skip)
        {
          yield return new WaitForSeconds(speed * speedfactor);
        }
        source.Play();
      }
    }

    if(continued)
    {
      desiredline = dialogue.dialogues[index - 1].line + " " + dialogue.dialogues[index].line;
    }
    else
    {
      desiredline = dialogue.dialogues[index].line;
    }

    //button manager + continue without press
    if(CheckIndex(true))
    {
      if(dialogue.dialogues[index + 1].advanced.continuewithoutpress)
      {
        yield return new WaitForSeconds(buttonpausetime);
        continueactive = true;
        Continue();
      }
      else if(auto)
      {
        yield return new WaitForSeconds(buttonpausetime + autotime);
        continueactive = true;
        Continue();
      }
      else
      {
        yield return new WaitForSeconds(buttonpausetime);
        continueactive = true;
        continuebutton.interactable = true;
      }
    }
    else
    {
      yield return new WaitForSeconds(buttonpausetime);
      continueactive = true;
      continuebutton.interactable = true;
    }
  }


  void SkipLine()
  {
    speedfactor = 0f;
    skip = true;
  }



  public bool CheckIndex(bool notlast)
  {
    if(notlast)
    {
      return(index < dialogue.dialogues.Length - 1);
    }
    else
    {
      return(index > 0);
    }
  }


  void SetSprite(Sprite target)
  {
    character.sprite = target;
  }



  //to the next line
  public void Continue()
  {
    if(continueactive)
    {
      if(index < dialogue.dialogues.Length - 1)
      {
        index++;
        if(!dialogue.dialogues[index].advanced.continued)
        {
          maintext.text = "";
          desiredline = dialogue.dialogues[index].line;
          StartCoroutine(TypeLine(false));
        }
        else
        {
          if(index > 0)
          {
            if(dialogue.dialogues[index - 1].charactercode == dialogue.dialogues[index].charactercode)
            {
              maintext.text += " ";
              desiredline = dialogue.dialogues[index - 1].line + " " + dialogue.dialogues[index].line;
              StartCoroutine(TypeLine(true));
            }
            else
            {
              maintext.text = "";
              desiredline = dialogue.dialogues[index].line;
              StartCoroutine(TypeLine(false));
            }
          }
          else
          {
            maintext.text = "";
            desiredline = dialogue.dialogues[index].line;
            StartCoroutine(TypeLine(false));
          }
        }
      }
      else
      {
        index = 0;
        maintext.text = "";
        desiredline = dialogue.dialogues[index].line;
        StartCoroutine(TypeLine(false));
      }
    }
    else
    {
      SkipLine();
    }
  }


  public void AutoType()
  {
    if(!auto)
    {
      auto = true;
      autobutton.sprite = autoactivesp;
      if(continueactive)
      {
        Continue();
      }
    }
    else
    {
      auto = false;
      autobutton.sprite = autoinactivesp;
    }
  }


  void CheckChar(char letter)
  {
    char[] letters = dialogue.dialogues[index].line.ToCharArray();
    int letterindex = Array.FindIndex(letters, letters => letters == letter);
    foreach(FormatList letter1 in chars)
    {
      if(letter == letter1.triggerchar)
      {
        letter1.charactive = true;
        CheckChar(letters[letterindex + 1]);
      }
    }
  }

  bool CheckSkip(char letter)
  {
    foreach(FormatList letter1 in chars)
    {
      if(letter == letter1.triggerchar)
      {
        return (true);
      }
    }
    return(false);
  }
}
