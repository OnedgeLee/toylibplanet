Toylibplanet
============

## Requirements

- `Libplanet.Crypto` : Used as implementation of ECDSA
  - `Libplanet.Crypto.PrivateKey`
  - `Libplanet.Crypto.PublicKey`

## Lacked

- Deserialization is not supported
  - So, cannot be used for network condition
  - For now, directly used instances assume that they're serialized and deserialized
  - Serialization is implemented just to generate hash
- Do not support storage
  - Since deserialization is not supported, it's meaningless to store date on storage
  - It's seems to be easy to save serialized instances to storage, if not on distributed condition
- IAction and IStates are not actually interfaces
  - I've misunderstood about interface, and implemented them as abstract classes

## Paid attention to

- Why libplanet is made like that
  - On verification stage, what is needed and what is not
  - Which informations are need to be hashed, and why

## How to test functionalities

- `Toylibplanet.Tests.ClientScenarioTest` contains scenario test assume simplified situation
- With this test, can run toylibplanet on single-node condition, and figure out if functions are working properly