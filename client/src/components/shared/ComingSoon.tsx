import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { PageHeader } from './PageHeader'

export function ComingSoon({ title, description }: { title: string; description: string }) {
  return (
    <div>
      <PageHeader title={title} description={description} />
      <Card className="border-dashed">
        <CardHeader>
          <CardTitle className="text-base font-medium text-muted-foreground">Coming soon</CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          This section is on the roadmap for an upcoming milestone. The navigation entry, route and API
          are already wired up - the full UI will land here next.
        </CardContent>
      </Card>
    </div>
  )
}
