```mermaid
---
  config:
    class:
      hideEmptyMembersBox: true
---
classDiagram
direction LR

namespace Classes.Security {
  class Bcrypt
  class Sha256 {
    <<static>>
  }
}

Bcrypt --> Sha256
```