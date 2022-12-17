import { Stack } from "@mui/material";
import NextLink from "next/link";
import { memo } from "react";
import { PricingNode } from "../../gql/codegen/graphql";
import { Symbol } from "./symbol.component";
import { Price } from "./price.component";
import { Change } from "./change.component";

type TickerItemProps = {
    pricingNode: PricingNode;
};

export const TickerItem = memo<TickerItemProps>(function TickerItem(props: TickerItemProps) {
    const { pricingNode } = props;

    return (
        <NextLink href={`/nodes/${pricingNode.regionalTransmissionOperatorId}/${pricingNode.name}`}>
            <Stack direction="column" spacing={1} minWidth={100}>
                <Symbol pricingNode={pricingNode} size="tiny" showRto={true} />
                <Price value={pricingNode.currentPrice} options={{ currency: "USD" }} />
                <Change value={pricingNode.change24Hour / 100} size="tiny" />
            </Stack>
        </NextLink>
    );
});
