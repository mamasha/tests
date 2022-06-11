interface Msg {
    "--start-op": {
        left: Number
        right: Number
        op: string
        ans: Number
    },
}

interface IMsgBus {
    on<Name extends keyof(Msg)>(name: Name, cb: (args: Msg[Name]) => void): this;
    fire<Name extends keyof(Msg)>(name: Name, args: Msg[Name]): this;
}

class MsgBus implements IMsgBus {
    on<Name extends keyof(Msg)>(name: Name, cb: (args: Msg[Name]) => void): this {
        return this;
    }

    fire<Name extends keyof(Msg)>(name: Name, args: Msg[Name]): this {
        return this;
    }
}

export let msgBus: IMsgBus = new MsgBus();

