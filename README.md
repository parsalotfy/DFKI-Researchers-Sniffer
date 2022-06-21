# DFKI-Researchers-Sniffer


This project crawls the [FORSCHUNGSBEREICHE](https://www.dfki.de/web/forschung/forschungsbereiche) webpage to retrieve all the researchers working in each department.

Then it crawls the [PUBLIKATIONEN](https://www.dfki.de/web/forschung/projekte-publikationen/publikationen) webpage to retrieve all the papers and people who have done them.
And store them all in a SQLite database

Researchers Table:

| Email(key)         | Name          | Department               |
| -------------      | ------------- | --------                 |
| Tom@dfki.com       | Tom           | Eingebettete Intelligenz |
| Bob@rhrk.uni-kl.de | Bob           | Interaktive Textilien    |
| ...                | ...           | ...                      |

<br></br>
HasPublications Table:
| id(key)            | Email(foreign key) | PublicationName                |
| -------------      | -------------      | --------                       |
| 6k786g6cg6866zf7   | Tom@dfki.com       | A paper about Covid            |
| 43iu5h34i5h34ih5   | Bob@rhrk.uni-kl.de | A paper about Virtual Reality  |
| ...                | ...                | ...                            |

Using these 2 tables we will match each publication author with the corresponding researcher and it's department.
So we could be able to see other papers of that author.


This project contains a Backend and Frontend.

The Frontend is done using Electron by [@HoussemEJ](https://github.com/HoussemEJ) and the Backend is done using C# by [@ParsaLotfy](https://github.com/parsalotfy)

# Screenshots

![WhatsApp Image 2022-06-21 at 4 19 23 PM (2)](https://user-images.githubusercontent.com/30390810/174823211-6e582a6a-892c-465d-8771-b0dc3b8eeb8a.jpeg)

![WhatsApp Image 2022-06-21 at 4 19 23 PM](https://user-images.githubusercontent.com/30390810/174823116-ca999b11-633d-4eac-bdbc-a87ff7091926.jpeg)

![WhatsApp Image 2022-06-21 at 4 19 23 PM (1)](https://user-images.githubusercontent.com/30390810/174823151-719ef83f-d835-4baa-bf53-de0f08431ac4.jpeg)

# Recording

https://user-images.githubusercontent.com/30390810/174823902-08fa5b13-ac85-4a4d-80b2-4ef8f161f6c4.mp4


