<script lang="ts">
    import Flex from './Flex.svelte'
    import Num from './Num.svelte'
    import Op from './Op.svelte'
    import Ans from './Ans.svelte'
    import {msgBus} from './MsgBus'

//    msgBus.fire("--test", {});
</script>

<Flex>
    <Num num={27}/>
    <Op op={'+'}/>
    <Num num={14}/>
</Flex>

<hr>

<Flex>
    <Ans ans={51}/>
    <Ans ans={31}/>
    <Ans ans={41}/>
</Flex>
