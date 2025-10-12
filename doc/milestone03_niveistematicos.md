# Milestone 03
## Informações Gerais
Entrega: 08/10/2025 <br>
Status: Concluído <br>
Integrantes:
- Daniel Barroquelo
- Gabriel Mezet
- Giovana Couto
- Júlia Velozo
- Pedro Terezzino
- Yuri Telis <br>

## Proposta
- Três níveis temáticos interconectados (passado, presente e futuro) <br>

Nos foi solicitado que neste ponto do projeto fosse entregue três niveis temáticos obrigatórios do nosso jogo (passado, presente e futuro) com navegação, desta forma, neste documento estão os seguintes itens:
- Diagrama de navegação entre cenários/fases.
- Imagens da implementação de cada um dos cenários.
- Explicação de como puzzle e progressão narrativa são afetados pelas mudanças temporais.
- Descrição do fluxo narrativo entre cenas/fases. <br>

## Níveis Temáticos
### Diagrama de Navegação
A seguir, terá o _level design_ do jogo focando mais no formato da o mapa e não na funcionalidade de cada um dos mapas.

#### Andar Inferior
<img width="1890" height="1417" alt="Andar_Inferior" src="https://github.com/user-attachments/assets/d95ec43e-924b-4256-8c65-b892a3146676" /> <br>

#### Andar Superior
<img width="1890" height="1417" alt="Andar_Superior" src="https://github.com/user-attachments/assets/fa484606-00d5-4fe3-b40e-254b1453e72e" /> <br>

### Como a Temporalidade Afeta a Gameplay?
A viagem no tempo afeta os puzzles e a progressão narrativa/_gameplay_ de forma que o jogador será "obrigado" a viajar no tempo para descobrir pistas em certas salas, como ver um eco do passado em uma sala no presente ou futuro e ter que viajar para o passado nessa sala específica para achar um elemetno que pode dar pista da história do jogo ou de algum puzzle.

### Fluxo Narrativo entre Cenas
**Passado:** o passado do mapa se passa no período vitoriano e é o momento onde a mansão estava no seu auge, sendo habitada e com pessoas transitando por ela. <br>
**Presente:** o presente a mansão está em processo de degradação por conta de todo o tempo que passou inabitada e sem manutenção. <br>
**Futuro:** o futuro é a mansão em um estado pior, por não ter sido habitada em muito tempo, e sendo influenciada pela viagem temporal. <br>
A protagonista viaja por essas temporalidades para descobrir mais sobre o passado da mansão e daqueles que um dia a habitaram, interagindo com puzzles e personagens que dão suporte à sua investigação.

### Implementação
Abaixo, estão a implementação em nível atual do que vai se tornar o cenário do jogo. No momento, os _tiles_ do presente estão mais avançados na produção e os que pertencem às outras temporalidades, ainda estão no início do processo. Além disso, a cena utilizada para tal, é um teste para verificação do que está pronto e o que ainda precisa ser feito ou alterado. Desta forma, as imagens abaixo ainda não são as versões finais do mapa do jogo, apenas uma concept do cenário.

#### Passado
<img width="340" height="350" alt="Implementação inicial das salas no passado" src="https://github.com/user-attachments/assets/ae09c7cb-46db-46af-9a0b-0eec6bc2bf88" />

#### Presente
<img width="338" height="552" alt="Implementação inicial das salas no presente" src="https://github.com/user-attachments/assets/00535383-b0e7-4a2b-b357-9d93e81af2cc" />

#### Futuro
<img width="337" height="351" alt="Implementação inicial das salas no futuro" src="https://github.com/user-attachments/assets/eea82e8d-b90e-4354-bb9d-7b166f521c06" />
