# Milestone 04
## Informações Gerais
Entrega: 15/10/2025 <br>
Status: Em andamento <br>
Integrantes:
- Daniel Barroquelo
- Gabriel Mezet
- Giovana Couto
- Júlia Velozo
- Pedro Terezzino
- Yuri Telis <br>

## Proposta
- Boss, estrutura de esqueleto, menu, intro e seleção <br>

Para esta entrega, nos foi solicitado que fossem desenvolvidas a navegação (menu principal e tela de introdução) e a boss fight. Desta forma, neste arquivo está documentado desenvolvimento destas funcionalidades do jogo.

## Estrutura de Navegação e Navegação
### Estrutura
<img width="1850" height="840" alt="diagrama que representa a navegação interna do jogo feito com ovais representando as telas e setas representando o caminho" src="https://github.com/user-attachments/assets/26d209d1-8e8c-4d31-9b10-054e90ddb6e9" /> <br>
#### Descrição das Telas e Seus Caminhos
- *__Menu principal:__* A tela principal irá conter uma imagem de fundo, o título do jogo e 4 (quatro) botões que irão levar o jogador ao jogo, tela de opções, tela de créditos e sair do jogo, respectivamente. 
- *__Opções:__* Este botão leva a tela de opções que permitirá o jogador personalizar algumas configurações, como o volume da música, e terá um botão que permitirá o jogador voltar para o menu.
- *__Créditos:__* Este botão leva a tela de créditos que permitirá o jogador ver os desenvolvedores do jogo e suas funções e terá um botão que permitirá o jogador voltar para o menu.
- *__Sair:__* Este botão permite o jogador fechar o jogo.
- *__Jogar:__* Este botão permite que o usuário acesse o jogo.
- *__Pause:__* quando o jogador pressionar esc enquanto estiver jogando, o jogo será pausado e terá 3 botões, o primeiro para voltar a jogar, o seugndo para acessar as onfigurações do jogo e o terceiro para voltar ao menu. <br>

### Navegação Implementada
#### Tela de Título
<img width="1757" height="836" alt="Captura de tela 2025-10-15 130406" src="https://github.com/user-attachments/assets/a149c45e-f4dc-4395-9634-8d49598df2eb" /> <br>
#### Tela de Opções
<img width="1756" height="831" alt="Captura de tela 2025-10-15 130415" src="https://github.com/user-attachments/assets/5d10a724-b89e-4733-94ad-e73c2b690c65" /> <br>
#### Tela de Créditos
<img width="1758" height="836" alt="Captura de tela 2025-10-15 130514" src="https://github.com/user-attachments/assets/5a5ebed4-0572-4381-aef5-507047ccd88f" /> <br>
#### Tela de Pause
<img width="1919" height="836" alt="Captura de tela 2025-10-15 130529" src="https://github.com/user-attachments/assets/9fa191a1-a555-4ab8-8ee4-ecb15046cae7" /> <br>


## Mecânicas do Boss e Boss Fight
### Mecânicas
- *__Ataque básico:__* O ataque básico do lobo consiste em um retângulo vermelho que irá mostrar o caminho que o lobo irá percorrer para acertar o jogador, desta forma o jogador precisa desviar para não receber dano. <br>
<img width="671" height="1220" alt="esta imagem mostra o ataque básico do lobo, que consiste em um retângulo vermelho que mostra o caminho pelo qual o lobo seguirá para avançar no jogador" src="https://github.com/user-attachments/assets/94d5b695-4297-4458-b90d-1f3967b44d2e" /> <br>
- *__Ataque de Fogo:__* O ataque de fogo consiste em faixas horizontais e verticais que mostrarão o caminho que as bolas de fogo irão percorrer para o jogador poder desviar. <br>
<img width="671" height="1220" alt="esta imagem mostra o ataque de fogo do lobo, que consiste em faixar verticais e horizontais que mostram o caminho que o fogo percorrerá" src="https://github.com/user-attachments/assets/81f5f572-76a1-4673-a941-20963ac941cb" /> <br>

### Implementação
#### Ataque Básico
<img width="1220" height="671" alt="esta imagem mostra o ataque básico do lobo, que consiste em um retângulo vermelho que mostra o caminho pelo qual o lobo seguirá para avançar no jogador" src="https://github.com/user-attachments/assets/63b36dba-731d-4b71-8c93-f9de956cabfb" /> <br>

#### Ataque de fogo
<img width="1221" height="686" alt="esta imagem mostra outro ataque do lobo, o qual exibem faixas horizontais e verticais e a protagonista não pode ficar no caminho se não será atacada" src="https://github.com/user-attachments/assets/a9cd6667-3805-4b45-952d-fb39b927ea25" /> <br>
<img width="1214" height="681" alt="Captura de tela 2025-10-15 112742" src="https://github.com/user-attachments/assets/96a05f4e-5408-4a75-97d3-69f11f6f0fc8" /> <br>
