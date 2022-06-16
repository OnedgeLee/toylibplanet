Toylibplanet
============

Since the purpose of this implementation is onboarding, there are several policies that i've made up.
1. Write `README.md` as procedual idea concretization note (It's a note rather than a project document)
2. Use `Libplanet` as a library, if there are no concrete reason (Avoid implementation from scratch)
3. Concentrate on macro behaviors. For micro behaviors, investigate them if there are remaining time. Don't be too ambitious.
4. Always rememver, my first goal is to know what functionality is where in Libplanet.
5. First target is to implement blockchain properly, and run single node, just on memory.
6. Second target is to make use of permenant memory.
7. Extend blockchain with some communication methods, to cover more nodes.

DO NOT PAY TOO MUCH TIME FOR BEAUTIFUL DOCUMENTATION, IT'S NOTHING BUT AN EXERCISE.
CONCENTRATE ON UNDERSTANDING LIBPLANET.

## Requirements

- `Libplanet.Crypto` : Used as implementation of ECDSA
  - `Libplanet.Crypto.PrivateKey`
  - `Libplanet.Crypto.PublicKey`
- `Libplanet` : Used as implementation of SHA256
  - `Libplanet.HashAlgorithmType`
  - `Libplanet.HashDigest`
  - `Libplanet.Address`
- `Libplanet` : Used as implementation of protocol
  - `Libplanet.TxMetadata`
- `Bencodex` : Used for bencodex serialization

## Design detail

Transaction class on `libplanet-python` has attributes below.

Transaction
  - attributes
    - sender(Address)
    - publicKey(PublicKey)
    - signature(bytes)
    - recipient(Address)
    - actions(Sequence[Action])
    - timestamp(DateTime)

But, do we really need attributes `sender` and `recipient`?
Bitcoin have `sender` and `recipient`, since it's intended to record history of "sending".
But how about game?
`sender` may be needed, since it indicates the subject of action.
But `recipient` seems not to be needed, so I've decided to remove `recipient` attribute.

For `sender` attribute, careful investigatation is needed.
Actually, we can add the subject of action in action class, instead of holding `sender` attribute.
So, I can express two options like below:
- Maintain `actor` attribute on the `Transaction` class
- Maintain `actor` attribute on the `Action` class
I would express `sender` as `actor`, as a subject of action.

For current design, subjects of actions on same transaction are same, so it seems to be better to maintain `actor` on `Transaction` class.

Q : For real-time MMO, do we really need to separate sequence of actions by its actor? If blockchain is nothing but an records, can't we make a usage of bunch of action sequences?
On the condition that we group some players, and let them play as just a kind of p2p net game, and record action information at the end of the group game,
They will share actions from different players, so at that time, individual action needs actor.
Thinking more about it, as a result, there are some transactions with same actions that contains different `publicKey`s and `signature`s.
Is it okay to handle them? How can we handle duplication of action? Do we have to generate single `Transaction`?
In fact, action checking among group is needed, not to someone modifies action of his group peers.
So, it seems to be profitable each individual in group generates transactions.
It seems to need several additional logics that confirming transactions between group members and broadcasting information is needed for this.

Q : Another question. It seems really hard to capture some kind of macro.
Malicious state transition is easy to capture, but how about cheat actions?
Since we don't have any central server, each peer client have to do that, but it means anti-cheat logic is opened to public, make it easy to breakthrough it.

I'm not sure about the idea, and need to ask, but for now it seems to put `actor` on `Action` class rather than put it on `Transaction` class.

So, My structure of of Transaction class is like...
But now I'm wondered if it's really. Are there also some transactions that are from bitcoin trading?
If so, sender and recipient are also needed...
Is it bi-purpose...?
--> For 9c, it's bi-purpose

Transaction
  - attributes
    - publicKey(PublicKey) : Public key of actor (may be extended to public key of observer?)
    - signature(bytes) : Signature of actor (may be extended to public key of observer?)
    - actions(Sequence[Action]) : Sequence of actions
    - timestamp(DateTime) : Time that transaction has been generated -> Used to secure sequence of transaction?
  - methods
    - make
      - functionality : Get privateKey, actions, timestamp and generate publicKey, address and signature, generates Transaction
      - Do we really need to use address? wouldn't be public key is enough? Just for checksum?
      - May be it's purpose of hiding public key to secure anonymity.
    - id
      - functionality : Generate TxId with Transaction payload
    - validate
      - functionality : With publicKey, signature, and data, verify if signature is valid
      - Actually doesn't need to check addresses, since it's not related to spending. 
    - serialize
      - functionality : Serialize transaction to later usage on messaging
    - bencode
      - functionality : Bencodex serialization



Block
  - attributes
    - index(int) : Block index
    - difficulty(int) : Mining difficulty
    - nonce(Nonce) : Nonce that is mined
    - rewardBeneficiary(Address) : Address of miner, to give reward satoshi
    - previousHash(Hash) : Hash of previous block
    - timestamp(Datetime) : Time that block has been generated
    - transactions
  - methods
    - mine
      - functionality : mining
      - For answer function in HashCash, I think it would be better to implement it directly
    - hash
      - functionality : Get hash of current block
    - serialize
      - functionality : Serialize block to later usage on messaging
    - bencode
      - functionality : Bencodex serialization
    - validate
      - functionality : Block validation

BlockChain
  - attributes
    - store(Store) : Store that blocks are saved. I won't save them on disk. They will be alive on memory
    - blocks : Mapping table of [Hash -> Block]
    - transactions : Mapping table of [TxId -> Transaction]
    - addresses : Mapping table of [Addresses -> Sequence[Transaction]]
  - methods
    - get
      - functionality : Get certain block
    - getStates
      - functionality : From certain addresses and offset(If there not, starts from last block), get (Address, State) sequence toward genesis block with time-reversed order
      For more detail, it gets adresses and use it as key, and fill their values as states
    - append
      - functionality : Append a Block to current Blockchain and validate
    - evaluateActions
      - functionality : Given Block, fetch State sequence to previous state
      - It iterates actions and make use of two addresses - sender and recipient, and if both of addresses are not in updated to State sequence yet, it adds information.
      - But we don't need state of recipient actually, Since there are not 'recipient'.
      - How about using State of sender? It doesn't seem to appropriate either, Since each action can affect to not states not just of 'sender', but other states.
      - Action have to indicate target states that will be changed, and it will be implemented by game developer.
    - exepectDifficulties
      - functionality : Gradually increases difficulties
    - validate
      - functionality : Validate block
    - stage_transactions
      - functionality : Update transaction of current block
    - mine_block
      - functionality : Mine block with stored transactions and append it on chain

---

Here is end of target 1

Additional notes will be written after target 1 achieved.