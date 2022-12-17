import { Stack } from "@mui/material";
import { PricingNode, Rtos } from "../../gql/codegen/graphql";
import { Symbol } from "../ticker/symbol.component";
import { Change } from "../ticker/change.component";
import { Price } from "../ticker/price.component";
import { memo } from "react";

type NodeProps = {
    pricingNode: PricingNode;
};

export const Node = memo<NodeProps>(function Node(props: NodeProps) {
    const { pricingNode } = props;

    return (
        <table style={{ width: "100%" }}>
            <tbody>
                <tr>
                    <td>
                        <Symbol pricingNode={pricingNode} size="large" />
                    </td>
                    <td style={{ width: "100%", textAlign: "right" }}>
                        <Stack direction="column" spacing={1} minWidth={100}>
                            <Price value={pricingNode.currentPrice} options={{ currency: "USD" }} />
                            <Change value={pricingNode.change24Hour / 100} size="tiny" />
                        </Stack>
                    </td>
                </tr>
            </tbody>
        </table>
    );
});
