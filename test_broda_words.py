import requests
import time


"""
This is to solve a very specific puzzle that was cute but after a while I was done with it.
You had to provide a word from the broda word list (found on the interwebz) meant for solving
crosswords, and a series of tests would run and either pass or fail. Through induction you had
to figure out what the tests were testing and then find a word that would pass all the tests.

This code takes the rules that we had figured out by hand and then loads the list of words and
if the word passes all the tests we know it should, try it on the web endpoint (simple form submission)
"""

def word_score(word):
  total = 0
  for c in word:
    total += ord(c) - ord('A') + 1
  
  return total

def count_uniques(word):
  letter_count = {}
  for c in word:
    if c in letter_count:
      letter_count[c] += 1
    else:
      letter_count[c] = 1

  uniques = 0
  for v in letter_count.values():
    if v == 1:
      uniques += 1
  
  return (uniques > 1)

def is_alternating(word):
  vowels = 'AEIOUY'
  next_is_vowel = word[0] in vowels
  for c in word:
    if (next_is_vowel and c in vowels) or (not next_is_vowel and c not in vowels):
      next_is_vowel = not next_is_vowel
      continue
    else:
      return False
  return True

url = "https://www.pandamagazine.com/island11/puzzles/randomtestingunit.php"
data = {"testword": "ZYMO"}

with open('broda_word_list.txt') as wordlist:
  matches = []
  for l in wordlist:
    l = l.strip()
    word = l.split(';')[0]
    
    if len(word) != 4 and len(word) != 9 and len(word) != 16:
      continue
    if word.find('I') > -1:
      continue
    if word.find('M') < 0:
      continue
    if word_score(word) < 40:
      continue
    if not count_uniques(word):
      continue
    if not is_alternating(word):
      continue

    matches.append(word)

matches.sort()
for m in matches:
  data['testword'] = m
  response = requests.post(url, data)
  if not response.ok:
    print('Failed fetching results with word ' + m)
    print(response)
  else:
    error_count = response.text.count('Result is FAILURE')
    print(f"{m} had {error_count} failures.")
    if error_count == 0:
      break
  time.sleep(.25)
  
