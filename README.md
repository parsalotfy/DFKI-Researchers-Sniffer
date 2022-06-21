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
