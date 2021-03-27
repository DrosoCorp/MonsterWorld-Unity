[Confluence Page](https://utopics.atlassian.net/wiki/spaces/MONSTERWOR/pages/17039587/Technical+Overview)

# Technical Overview

![MonsterWorld Logo](https://noe.masse.pro/img/monsterworld.png)

## Summary

```
Monster World is an online multiplayer collectible monster game, with both an overworld real-time gameplay and a turn-based battle system.  
The players have to capture, train and rephase their monsters to compete in tournaments.
```

## Tools

### Hardware

| Minimal Configuration | Windows                                      | MacOS                          |
|-----------------------|----------------------------------------------|--------------------------------|
| OS Version            | Windows 7 (SP1+), 64-bit, Windows 10, 64-bit | Sierra 10.12.6+                |
| CPU                   | x64 with SSE2 instruction set                | x64 with SSE2 instruction set  |
| Graphics              | Compatible DX10, DX11 & DX12                 | Compatible Metal (Intel & AMD) |

### Software

| Product                 | Usage                                        |
|-------------------------|----------------------------------------------|
| Unity 2019.4.11f1 (LTS) | Game Engine & Editor                         |
| Git                     | Versioning                                   |
| Blender 2.8+            | Modeling, Rigging, Animating                 |
| Adobe CC                | Concept Art, Painting, Texturing, UI         |


## Development Environment

> :information_source: If you have any question or feedback about the development environment, refer to the *Technical Director*.

### Versioning

* Source code versioning is done using Git.
* Assets versioning is done using Git LFS.
* The repository is hosted on Github.

### Workflow

For the source code, the project follow the Gitflow workflow.

The complete process is described in the  [:page_facing_up: Development Workflow](https://utopics.atlassian.net/wiki/spaces/MONSTERWOR/pages/14221456/Development+Workflow) page.

### Project Structure

```
📂Assets
 ┣ 📂External
 ┣ 📂Client
 ┣ 📂Common
 ┃  ┗ 📂Inventory
 ┣ 📂Server
 ┗ 📂_YourFolder
    ┣ 🧊Monster021.prefab
    ┣ 📜RephaseAnimationScript.cs
    ┗ 🪐RephaseSceneTest.scene
```

For an in-depth explanation of the project structure, refer to the [:page_facing_up: Project Structure](https://utopics.atlassian.net/wiki/spaces/MONSTERWOR/pages/14221470/Project+Structure) page.

### Code Guidelines

For code guidelines, refer to the [:page_facing_up: Code Guidelines](https://utopics.atlassian.net/wiki/spaces/MONSTERWOR/pages/14221463/Code+Guidelines) page.

For good practices and avoid potential pitfalls, read the [:page_facing_up: Do's and Don'ts](https://utopics.atlassian.net/wiki/spaces/MONSTERWOR/pages/25231482/Do%27s+and+Don%27ts) page.