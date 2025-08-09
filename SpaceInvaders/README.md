# Space Invaders - Desenvolvido por Alisson Silva   

## Descrição do Projeto
Este projeto é uma recriação do clássico jogo **Space Invaders**. Ele foi desenvolvido utilizando **C#** e o framework **Avalonia**, seguindo o padrão **MVVM (Model-View-ViewModel)** para melhor separação da lógica do jogo e da interface gráfica.

## 👤 Autor
- **Alisson Silva** – Desenvolvimento completo (lógica, interface, sons, organização do código e integração)

## Funcionalidades
- 🎮 Controle do jogador com teclado
- 👾 Inimigos especiais
- 💥 Disparos e colisões
- 🔊 Efeitos sonoros
- 🏆 Sistema de pontuação

## Tecnologias Utilizadas
- **Linguagem:** C#
- **Frameworks/Libraries:**
  - **Avalonia**: Framework para a interface gráfica, similar ao WPF.
  - **DispatcherTimer**: Utilizado para controlar a atualização do jogo.
- **Git e GitLab**

## Como Executar o Projeto
1. Clone o repositório:
   ```bash
   git clone https://github.com/AlinhoZz/space_invaders.git
   ```
2. Acesse o diretório do projeto:
   ```bash
   cd /home/user/pasta/spaceinvaders/SpaceInvaders
   ```
3. Execute o jogo:
   ```bash
   dotnet build
   dotnet run
   ```

## Estrutura do Projeto
```
📁 SpaceInvaders
├── 📂 SpaceInvaders
│   ├── 📂 Dependencies
│   ├── 📂 Assets
│   ├── 📂 Controllers
│   │   ├── 🎮 BunkerController.cs
│   │   ├── 🎮 EnemyController.cs
│   │   ├── 🎮 GameController.cs
│   │   ├── 🎮 PlayerController.cs
│   │   ├── 🎮 ScoreController.cs
│   │   ├── 🎮 ShotsController.cs
│   │   ├── 🎮 UFOController.cs
│   ├── 📂 Models
│   │   ├── 🎯 Bullet.cs
│   │   ├── 🏰 Bunker.cs
│   │   ├── 👾 Enemy.cs
│   │   ├── 🚀 Player.cs
│   │   ├── 🛸 UFO.cs
│   ├── 📂 Services
│   │   ├── 🎵 LoopStream.cs
│   │   ├── 🎵 SoundManager.cs
│   ├── 📂 ViewModels
│   ├── 📂 Views
│   │   ├── 📄 CustomMessageBox.cs
│   │   ├── 📄 GameWindow.axaml
│   │   ├── 📄 MainWindow.axaml
│   ├── 📄 Program.cs
├── 📄 README.md
├── 📄 SpaceInvaders.csproj
└── 📄 .gitignore

```

## Licença
Este projeto é código aberto e pode ser utilizado livremente para estudo, modificação e aprimoramento.

## Repositório Oficial
- [📂 Acesse o projeto no GitLab](https://github.com/AlinhoZz/space_invaders.git)

## Contato
Caso tenha dúvidas ou sugestões, entre em contato com os desenvolvedores através de seus perfis no GitLab:
- [Alisson Silva](alissondurden@gmail.com)
