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
- Time problem
  - Found timestamp is not working as I expected
  - May be TimeStampOffset.Now does not generate actual timestamp
  - Can be fixed just saving not instance, but its ticks

## Paid attention to

- Why libplanet is made like that
  - On verification stage, what is needed and what is not
  - Which informations are need to be hashed, and why